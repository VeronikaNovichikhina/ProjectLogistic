using LogisticCompany.Application.Interfaces;
using LogisticCompany.Application.DTO;
using LogisticCompany.Db;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LogisticCompany.Application.Services
{
    public class OpenStreetMapService : IMapService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _db;

        // Страны где нет наземного маршрута между собой
        private static readonly HashSet<(int, int)> AirOnlyRoutes = new()
        {
            (1, 3), (3, 1),  // Казахстан  Китай
            (2, 3), (3, 2),  // Россия  Китай
        };

        public OpenStreetMapService(
            HttpClient httpClient,
            IConfiguration configuration,
            AppDbContext db)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _db = db;
        }

        // Проверка — только ли авиа для этого маршрута
        public bool IsAirOnlyRoute(int originCountryId, int destinationCountryId)
        {
            if (originCountryId == destinationCountryId) return false;
            return AirOnlyRoutes.Contains((originCountryId, destinationCountryId));
        }

        // Основной метод расчёта расстояния
        public async Task<decimal> GetDistanceAsync(
            int originTownId,
            int destinationTownId,
            int transportTypeId)
        {
            var originTown = await _db.Towns
                .FirstOrDefaultAsync(t => t.TownId == originTownId);
            var destinationTown = await _db.Towns
                .FirstOrDefaultAsync(t => t.TownId == destinationTownId);

            if (originTown == null || destinationTown == null)
                throw new Exception("Город не найден");

            if (originTown.Latitude == null || originTown.Longitude == null ||
                destinationTown.Latitude == null || destinationTown.Longitude == null)
                throw new Exception("Координаты города не указаны");

            // Авиа транспорт — всегда Гаверсинус
            if (transportTypeId == 2)
            {
                return CalculateHaversine(
                    originTown.Latitude.Value,
                    originTown.Longitude.Value,
                    destinationTown.Latitude.Value,
                    destinationTown.Longitude.Value);
            }

            // Наземный транспорт — OpenRouteService
            try
            {
                return await GetRouteDistanceAsync(
                    originTown.Latitude.Value,
                    originTown.Longitude.Value,
                    destinationTown.Latitude.Value,
                    destinationTown.Longitude.Value);
            }
            catch
            {
                // Fallback на Гаверсинус с коэффициентом
                var haversine = CalculateHaversine(
                    originTown.Latitude.Value,
                    originTown.Longitude.Value,
                    destinationTown.Latitude.Value,
                    destinationTown.Longitude.Value);
                return haversine * 1.25m;
            }
        }

        // Запрос к OpenRouteService API
        private async Task<decimal> GetRouteDistanceAsync(
            double originLat, double originLon,
            double destLat, double destLon)
        {
            var apiKey = _configuration["OpenRouteService:ApiKey"];
            var url = $"https://api.openrouteservice.org/v2/directions/driving-car" +
                      $"?api_key={apiKey}" +
                      $"&start={originLon},{originLat}" +
                      $"&end={destLon},{destLat}";

            await Task.Delay(500); // rate limiting

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            var distanceMeters = doc.RootElement
                .GetProperty("features")[0]
                .GetProperty("properties")
                .GetProperty("segments")[0]
                .GetProperty("distance")
                .GetDecimal();

            return Math.Round(distanceMeters / 1000, 2); // в километры
        }

        // Формула Гаверсинуса
        private static decimal CalculateHaversine(
            double lat1, double lon1,
            double lat2, double lon2)
        {
            const double R = 6371; // радиус земли в км
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return (decimal)Math.Round(R * c, 2);
        }

        private static double ToRad(double deg) => deg * Math.PI / 180;
    }
}