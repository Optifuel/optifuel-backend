using Api.Data;
using Api.Models.Entities;
using Api.Services.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using Api.ExceptionApi.MapBox;
using System;
using System.Collections.Generic;

namespace Api.Services.Repositories
{
    public class MapBox : IMapBox
    {
        private readonly HttpClient httpClient;
        private const string accessToken = "pk.eyJ1Ijoic2FudGFsMTIxMCIsImEiOiJjbTQ2dnRwM2UxOWcwMmtxeHRqd2ppZmhjIn0.PMYKUNf0nPFK5soI4Eu10w";
        private const string geocodingUrl = "https://api.mapbox.com/geocoding/v5/mapbox.places/";
        private const string directionsUrl = "https://api.mapbox.com/directions/v5/mapbox/driving/";
        private const double EarthRadiusKm = 6371;

        protected ApiDbContext _context;

        public MapBox(ApiDbContext context)
        {
            httpClient = new HttpClient();
            _context = context;
        }


        private Dictionary<string, List<string>> fuelMapping = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "benzina", new List<string> { "benzina", "verde speciale", "benzina shell v power", "verde", "benzina 102 ottani", "benzina speciale 98 ottani", "benzina speciale", "benzina 100 ottani", "benzina energy 98 ottani", "benzina wr 100" } },
            { "diesel", new List<string> { "diesel", "gasolio" ,"diesel hvo", "supreme diesel", "dieselmax", "gasolio hvo", "gasolio oro diesel", "hvolution", "gasolio prestazionale", "gasolio artico", "v-power diesel", "blu diesel alpino", "gasolio artico", "hi-q diesel", "gasolio alpino", "gasolio plus", "diesel hvo energy", "gasolio energy d", "hvo eco diesel", "e-diesel", "gasolio gelo", "gasolio ecoplus", "gasolio speciale", "hvo100", "blue diesel", "hvovolution", "excellium diesel", "gasolio bio hvo", "gasolio premium", "rehvo", "diesel shell v power", } },
            { "gpl", new List<string> { "gpl", "gnl" } },
            { "metano", new List<string> { "metano" } }
        };
        private bool MatchesFuelType(string dbFuelType, string searchFuelType)
        {
            return fuelMapping.TryGetValue(searchFuelType, out var variants) && variants.Contains(dbFuelType.ToLower());
        }

        public async Task<Coordinates> GetCoordinates(string city)
        {
            string url = $"{geocodingUrl}{city}.json?access_token={accessToken}";
            
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            GeoCodingResponse? geoCodingResponse = JsonSerializer.Deserialize<GeoCodingResponse>(content);
            
            if(geoCodingResponse == null)
                throw new Exception("Error al deserializar la respuesta de MapBox");

            Models.Entities.Feature feature = geoCodingResponse.features[0];
            Coordinates coordinates = new Coordinates
            {
                Latitude = feature.center[1],
                Longitude = feature.center[0]
            };

            return coordinates;
        }

        public async Task<Models.Entities.Route> GetPathByTown(double initLongitude, double initLatitude, double endLongitude, double endLatitude)
        {
            Coordinates startTownCoordinates = new Coordinates { Longitude = initLongitude, Latitude = initLatitude };
            Coordinates endTownCoordinates = new Coordinates { Longitude = endLongitude, Latitude = endLatitude };

            string url = $"{directionsUrl}{startTownCoordinates.Longitude.ToString(CultureInfo.InvariantCulture)},{startTownCoordinates.Latitude.ToString(CultureInfo.InvariantCulture)};{endTownCoordinates.Longitude.ToString(CultureInfo.InvariantCulture)},{endTownCoordinates.Latitude.ToString(CultureInfo.InvariantCulture)}?access_token={accessToken}&alternatives=true&geometries=geojson&language=en&overview=full&steps=true";

            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            DistanceResponse? distanceResponse = JsonSerializer.Deserialize<DistanceResponse>(content);

            if(distanceResponse == null)
                throw new DeserializarMapBoxException();

            Models.Entities.Route route = distanceResponse.routes.OrderBy(r => r.distance).FirstOrDefault();

            return route;
        }



        public async Task<Models.Entities.Route> GetPathMultiplePoint(List<Coordinates> listPoints)
        {
            if (listPoints == null || listPoints.Count < 2)
            {
                throw new NotEnoughPointsException();
            }

            // Costruisci la stringa delle coordinate concatenando i punti
            string coordinatesString = string.Join(
                ";",
                listPoints.Select(point => $"{point.Longitude.ToString(CultureInfo.InvariantCulture)},{point.Latitude.ToString(CultureInfo.InvariantCulture)}")
            );

            // Componi l'URL dinamico
            string url = $"{directionsUrl}{coordinatesString}?access_token={accessToken}&alternatives=true&geometries=geojson&language=en&overview=full&steps=true";

            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            DistanceResponse? distanceResponse = JsonSerializer.Deserialize<DistanceResponse>(content);

            if (distanceResponse == null)
            {
                throw new DeserializarMapBoxException();
            }

            Models.Entities.Route route = distanceResponse.routes.OrderBy(r => r.distance).FirstOrDefault();

            return route;
        }

    public async Task<(List<GasStationRegistry>, List<Coordinates>)> FindGasStation(Vehicle vehicle, double percentTank, List<Coordinates> listPoints)
        {
            if(listPoints.Count < 2)
                throw new NotEnoughPointsException();

            List<GasStationRegistry?> list = new List<GasStationRegistry?>();
            double consume = vehicle.ExtraUrbanConsumption;
            double tank = vehicle.LitersTank;

            double distancePercent = 0.75;
            double rangePercent = 0.15;
            Models.Entities.Route tempRoute = await GetPathMultiplePoint(listPoints);
            if(tempRoute.distance/1000 < tank * consume * percentTank / 100)
                throw new NotEnoughDistanceException();

            List<Coordinates> ordered_points = new List<Coordinates>();

            

            for (int i =1 ; i < listPoints.Count ; i++)
            {
                ordered_points.Add(listPoints[i-1]);

                List<GasStationRegistry?> gasStationSelected = new List<GasStationRegistry?>();
                double initLongitude = listPoints[i - 1].Longitude;
                double initLatitude = listPoints[i - 1].Latitude;
                double endLongitude = listPoints[i].Longitude;
                double endLatitude = listPoints[i].Latitude;


                Models.Entities.Route route = await GetPathByTown(initLongitude, initLatitude, endLongitude, endLatitude);
                route.distance = route.distance / 1000;
                if( route.distance > tank * consume * percentTank / 100){
                    var gasStationFiltedList = _context.GasStationRegistry.Include(u => u.GasStationPrices).ToList(); //FilterGasStation(route.geometry.coordinates.Select(p => (Coordinates)p).ToList());
                    try
                    {
                        list.AddRange(await searchStation(route, tank, consume, percentTank, distancePercent, rangePercent, distancePercent, rangePercent, vehicle.FuelType.ToLower(), gasStationSelected, gasStationFiltedList));
                    } catch(InvalidOperationException)
                    {
                        try
                        {
                            list.AddRange(await searchStation(route, tank, consume, percentTank, distancePercent - 0.25, rangePercent, distancePercent, rangePercent, vehicle.FuelType.ToLower(), gasStationSelected, gasStationFiltedList));
                        } catch(InvalidOperationException)
                        {
                            throw new NoGasStationFoundException();
                        }
                    }

                    if (!list.Any())
                    {
                        Console.WriteLine($"Nessuna stazione trovata tra {i - 1} e {i}");
                        throw new NoGasStationFoundException();
                    }

                    ordered_points.AddRange(gasStationSelected.Select(g => new Coordinates 
                    { 
                        Latitude = (double)g?.Latitude, 
                        Longitude = (double)g?.Longitude 
                    }));
                    Coordinates station = new Coordinates { Latitude = (double)list.Last().Latitude, Longitude = (double)list.Last().Longitude };

                    var dist = await  checkDistance(station, listPoints[i]);
                    percentTank = (tank - (dist/consume))/tank  * 100;
                }else{
                    percentTank = percentTank -((route.distance/consume)/tank * 100);
                    Console.WriteLine($"[DEBUG] percentTank fra {i-1} e {i} " + percentTank);
                }

            }
            ordered_points.Add(listPoints.Last());
            list = FilterByFuelTypeAndBestPrice(list, vehicle.FuelType.ToLower());
            return (list, ordered_points);
        }   


        private List<GasStationRegistry> FilterByFuelTypeAndBestPrice(List<GasStationRegistry> gasStationList, string fuelType)
        {
            foreach (var station in gasStationList)
            {
                // Filtra i prezzi per il carburante di interesse
                var filteredPrices = station.GasStationPrices
                    .Where(price => MatchesFuelType(price.FuelType, fuelType))
                    .OrderBy(price => price.Price) // Ordina per prezzo crescente
                    .Take(1) // Prendi solo il prezzo migliore
                    .ToList();

                // Sostituisci la lista originale con quella filtrata
                station.GasStationPrices = filteredPrices;
            }

            // Rimuovi le stazioni senza prezzi validi
            return gasStationList.Where(station => station.GasStationPrices.Any()).ToList();
        }

        public async Task<List<GasStationRegistry?>> searchStation(
            Models.Entities.Route route, 
            double tank, 
            double consume, 
            double percentTank, 
            double distancePercent, 
            double rangePercent, 
            double defaultDistancePercent, 
            double defaultRangePercent, 
            string fuelType, 
            List<GasStationRegistry?>? gasStationSelected, 
            List<GasStationRegistry> gasStationFiltedList
            )
        {
            double delete = percentTank < 25 ? 0 : consume * tank * distancePercent * percentTank / 100;
            double range = consume * tank * rangePercent * percentTank / 100;


            if(route.geometry.coordinates.Count==0)
                return new List<GasStationRegistry?>();



            List<Coordinates> points = route.geometry.coordinates.Select(p => (Coordinates)p).ToList();
            int indexDelete = 0;
            double totalDistance = 0;
            bool indexDeleteFound = false;
            for(int i = 0 ; i < points.Count - 1 ; i++)
            {
                double segmentDistance = CalculateDistance(points[i], points[i + 1]);
                totalDistance += segmentDistance;
                if(totalDistance >= delete && !indexDeleteFound)
                {
                    indexDeleteFound = true;
                    indexDelete = i;
                }
            }

            if(totalDistance < delete)
                return new List<GasStationRegistry?>();

            var listTemp = gasStationFiltedList
            .Where(g => checkPointInRange(
                (Coordinates)route.geometry.coordinates[indexDelete],
                new Coordinates { Longitude = (double)g.Longitude, Latitude = (double)g.Latitude }, 
                10))
            .Where(g => g.GasStationPrices.Any(u => MatchesFuelType(u.FuelType, fuelType)))
            .OrderBy(g => g.GasStationPrices.First(u => MatchesFuelType(u.FuelType, fuelType)).Price)
            .ToList();

            Console.WriteLine("[DEBUG] List Temp " + listTemp.Count );

            double dist = await checkDistance((Coordinates)route.geometry.coordinates[indexDelete], new Coordinates { Latitude = (double)listTemp.First().Latitude, Longitude = (double)listTemp.First().Longitude });
            while(dist > range)
            {
                listTemp.RemoveAt(0);
                dist = await checkDistance((Coordinates)route.geometry.coordinates[indexDelete], new Coordinates { Latitude = (double)listTemp.First().Latitude, Longitude = (double)listTemp.First().Longitude });

                if(listTemp.Count == 0)
                    throw new NoGasStationFoundException();
            }

            route.geometry.coordinates.RemoveRange(0, indexDelete);
            route.geometry.coordinates.Insert(0, new List<double> { (double)listTemp.First().Longitude, (double)listTemp.First().Latitude });
            gasStationSelected.Add(listTemp.First());
            percentTank = 100;
            try
            {
                await Task.Run(() => searchStation(route, tank, consume, percentTank, defaultDistancePercent, defaultRangePercent, defaultDistancePercent, defaultRangePercent, fuelType, gasStationSelected, gasStationFiltedList));
            } catch(InvalidOperationException) {
                try
                {
                    await Task.Run(() => searchStation(route, tank, consume, percentTank, defaultDistancePercent - 0.25, defaultRangePercent, defaultDistancePercent, defaultRangePercent, fuelType, gasStationSelected, gasStationFiltedList));
                } catch(InvalidOperationException)
                {
                    throw new NoGasStationFoundException();
                }
            }
            Console.WriteLine("[DEBUG] gasStationSelected " + gasStationSelected.Count);
            return gasStationSelected;
        }

        public List<GasStationRegistry> FilterGasStation(List<Coordinates> points)
        {
            double radius = 10;
            var stationsInRange = _context.GasStationRegistry.Include(u=> u.GasStationPrices).ToList();
            ConcurrentBag<GasStationRegistry> gasStationRegistry = new ConcurrentBag<GasStationRegistry>();

            Parallel.ForEach(points, new ParallelOptions { MaxDegreeOfParallelism = (Environment.ProcessorCount - 2) > 0 ? (Environment.ProcessorCount - 2) : Environment.ProcessorCount }, (point, state, index) =>
            {
                var pointStation = stationsInRange.Where(g => checkPointInRange(point, new Coordinates { Longitude = (double)g.Longitude, Latitude = (double)g.Latitude }, radius)).ToList();
                foreach(var station in pointStation)
                {
                    gasStationRegistry.Add(station);
                }
            });

            return gasStationRegistry.GroupBy(obj => obj.Id).Select(group => group.First()).ToList();
        }

        private bool checkPointInRange(Coordinates center, Coordinates point, double radius)
        {
            double distance = CalculateDistance(center, point);
            return distance <= radius;
        }

        private void GetPointsFromDistance(List<List<double>> coordinate, int distanceKm)
        {
            // Converte o percorso em uma lista de pontos Coordinate
            List<Coordinates> points = new List<Coordinates>();
            foreach(var coord in coordinate)
            {
                points.Add(new Coordinates{ Longitude = coord[0], Latitude = coord[1] });
            }

            Coordinates? pointKm = null;
            double totalDistance = 0; 
            for(int i = 0 ; i < points.Count - 1 ; i++)
            {
                double segmentDistance = CalculateDistance(points[i], points[i+1]);
                totalDistance += segmentDistance;
                if(totalDistance >= distanceKm) 
                {
                    pointKm = points[i];
                    break;
                }
            }
        }

        private async Task<double> checkDistance(Coordinates point1, Coordinates point2)
        {

            string url = $"{directionsUrl}{point1.Longitude.ToString(CultureInfo.InvariantCulture)},{point1.Latitude.ToString(CultureInfo.InvariantCulture)};{point2.Longitude.ToString(CultureInfo.InvariantCulture)},{point2.Latitude.ToString(CultureInfo.InvariantCulture)}?access_token={accessToken}&alternatives=true&geometries=geojson&language=en&overview=full&steps=true";

            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            DistanceResponse? distanceResponse = JsonSerializer.Deserialize<DistanceResponse>(content);

            if(distanceResponse == null)
                throw new DeserializarMapBoxException();

            Models.Entities.Route route = distanceResponse.routes.OrderBy(r => r.distance).FirstOrDefault();
            return route.distance/1000;
        }


        public double CalculateDistance(Coordinates point1, Coordinates point2)
        {
            var dLat = ToRadians(point2.Latitude - point1.Latitude);
            var dLon = ToRadians(point2.Longitude - point1.Longitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(point1.Latitude)) * Math.Cos(ToRadians(point2.Latitude)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distance = EarthRadiusKm * c; // Distance in kilometers

            return distance;
        }

        private double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
