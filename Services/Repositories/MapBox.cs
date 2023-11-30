using ApiCos.Data;
using ApiCos.Models.Entities;
using ApiCos.Services.IRepositories;
using GeoJSON.Net.Geometry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Drawing;
using System.Globalization;
using System.Text.Json;

namespace ApiCos.Services.Repositories
{
    public class MapBox : IMapBox
    {
        private readonly HttpClient httpClient;
        private const string accessToken = "pk.eyJ1Ijoic2FudGFsMTIxMCIsImEiOiJjbG5kbnVtcGgwNW9lMnNtamJwN2ozOWE3In0.4zudJU8wSqYaCFCEcN-b-g";
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
            Console.WriteLine(city);
            string url = $"{geocodingUrl}{city}.json?access_token={accessToken}";
            Console.WriteLine(url);

            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            GeoCodingResponse? geoCodingResponse = JsonSerializer.Deserialize<GeoCodingResponse>(content);
            if (geoCodingResponse == null)
                throw new Exception("Error al deserializar la respuesta de MapBox");

            Models.Entities.Feature feature = geoCodingResponse.features[0];
            Coordinates coordinates = new Coordinates
            {
                Latitude = feature.center[1],
                Longitude = feature.center[0]
            };

            return coordinates;
        }

        public async Task<List<List<double>>> GetPathByTown(string startTown, string endTown)
        {
            Coordinates startTownCoordinates = await GetCoordinates(startTown);
            Coordinates endTownCoordinates = await GetCoordinates(endTown);

            string url = $"{directionsUrl}{startTownCoordinates.Longitude.ToString(CultureInfo.InvariantCulture)},{startTownCoordinates.Latitude.ToString(CultureInfo.InvariantCulture)};{endTownCoordinates.Longitude.ToString(CultureInfo.InvariantCulture)},{endTownCoordinates.Latitude.ToString(CultureInfo.InvariantCulture)}?access_token={accessToken}&alternatives=true&geometries=geojson&language=en&overview=full&steps=true";
            Console.WriteLine(url);

            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            DistanceResponse? distanceResponse = JsonSerializer.Deserialize<DistanceResponse>(content);

            if(distanceResponse == null)
                throw new Exception("Error al deserializar la respuesta de MapBox");

            Models.Entities.Route route = distanceResponse.routes.OrderBy(r => r.distance).FirstOrDefault();
            FilterGasStation(route.geometry.coordinates);
            return route.geometry.coordinates;
        }

        public async Task FindGasStation(Vehicle vehicle, string startTown, string endTown)
        {
            List<List<double>> point = await GetPathByTown(startTown, endTown);
            double consume = vehicle.ExtraUrbanConsumption;
            double tank = vehicle.LitersTank;

            double minimum = consume * tank * 0.75;
            double distance = consume * tank * 0.1;

            int nSections = (int)(distance / minimum);
            if(nSections == 0)
                return;
        }

        public List<GasStationRegistry> FilterGasStation(List<List<double>> points)
        {
            double radius = 10;
            var stationsInRange = _context.GasStationRegistry.ToList();
            ConcurrentBag<GasStationRegistry> gasStationRegistry = new ConcurrentBag<GasStationRegistry>();

            Parallel.ForEach(points, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (point, state, index) =>
            {
                var pointStation = stationsInRange.Where(g => checkPointInRange((Coordinates)point, new Coordinates { Longitude = (double)g.Longitude, Latitude = (double)g.Latitude }, radius));
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


        public static double CalculateDistance(Coordinates point1, Coordinates point2)
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

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
