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

        public async Task<List<GasStationRegistry>> FindGasStation(Vehicle vehicle, double percentTank, List<Coordinates> listPoints)
        {
            if(listPoints.Count < 2)
                throw new NotEnoughPointsException();

            List<GasStationRegistry?> list = new List<GasStationRegistry?>();
            double consume = vehicle.ExtraUrbanConsumption;
            double tank = vehicle.LitersTank;

            double distancePercent = 0.75;
            double rangePercent = 0.15;

            for (int i =1 ; i < listPoints.Count ; i++)
            {
                List<GasStationRegistry?> gasStationSelected = new List<GasStationRegistry?>();
                double initLongitude = listPoints[i - 1].Longitude;
                double initLatitude = listPoints[i - 1].Latitude;
                double endLongitude = listPoints[i].Longitude;
                double endLatitude = listPoints[i].Latitude;


                Models.Entities.Route route = await GetPathByTown(initLongitude, initLatitude, endLongitude, endLatitude);
                route.distance = route.distance / 1000;
                var gasStationFiltedList = _context.GasStationRegistry.Include(u => u.GasStationPrices).ToList(); //FilterGasStation(route.geometry.coordinates.Select(p => (Coordinates)p).ToList());
                try
                {
                    list.AddRange(await searchStation(route, tank, consume, percentTank, distancePercent, rangePercent, distancePercent, rangePercent, vehicle.FuelType.ToLower(), gasStationSelected, gasStationFiltedList));
                } catch(InvalidOperationException e)
                {
                    try
                    {
                        list.AddRange(await searchStation(route, tank, consume, percentTank, distancePercent - 0.25, rangePercent, distancePercent, rangePercent, vehicle.FuelType.ToLower(), gasStationSelected, gasStationFiltedList));
                    } catch(InvalidOperationException e2)
                    {
                        throw new NoGasStationFoundException();
                    }
                }
                Coordinates station = new Coordinates { Latitude = (double)list.Last().Latitude, Longitude = (double)list.Last().Longitude };
                var dist = await  checkDistance(station, listPoints[i]);
                percentTank = (  (tank - (dist/consume))/tank ) * 100;
            }
            return list;
        }

        public async Task<List<GasStationRegistry?>> searchStation(Models.Entities.Route route, double tank, double consume, double percentTank, double distancePercent, double rangePercent, double defaultDistancePercent, double defaultRangePercent, string fuelType, List<GasStationRegistry?>? gasStationSelected, List<GasStationRegistry> gasStationFiltedList)
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

            var listTemp = gasStationFiltedList.Where(g => checkPointInRange((Coordinates)route.geometry.coordinates[indexDelete], new Coordinates { Longitude = (double)g.Longitude, Latitude = (double)g.Latitude }, 10))
                                                    .Where(g => g.GasStationPrices.Any(u => u.FuelType.ToLower() == fuelType))
                                                    .OrderBy(g => g.GasStationPrices.First(u => u.FuelType.ToLower() == fuelType).Price).ToList();

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
            } catch(InvalidOperationException e) {
                try
                {
                    await Task.Run(() => searchStation(route, tank, consume, percentTank, defaultDistancePercent - 0.25, defaultRangePercent, defaultDistancePercent, defaultRangePercent, fuelType, gasStationSelected, gasStationFiltedList));
                } catch(InvalidOperationException e2)
                {
                    throw new NoGasStationFoundException();
                }
            }
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
