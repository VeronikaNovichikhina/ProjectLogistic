namespace LogisticCompany.Services
{
    using System.Globalization;
    using System.Text.Json;

    using System.Globalization;
    using System.Text.Json;

    public class OpenStreetMapService
    {
        private readonly HttpClient _httpClient;

        public OpenStreetMapService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "DeliveryApp/1.0");
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<decimal> GetDistanceAsync(string fromCity, string toCity, int transportTypeId)
        {
            try
            {
                Console.WriteLine($"Расчет расстояния: {fromCity} -> {toCity}");
                Console.WriteLine($" Тип транспорта: {(IsAirTransport(transportTypeId) ? "Авиа ✈" : "Наземный ")}");

                // Для авиа используем расстояние по прямой
                if (IsAirTransport(transportTypeId))
                {
                    return await GetAirDistanceAsync(fromCity, toCity);
                }

                // Для наземного используем расчет с учетом дорог
                return await GetRoadDistanceAsync(fromCity, toCity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка: {ex.Message}");
                return 500; // значение по умолчанию при ошибке
            }
        }

        private async Task<decimal> GetAirDistanceAsync(string fromCity, string toCity)
        {
            try
            {
                var fromCoords = await GetCoordinatesAsync(fromCity);
                var toCoords = await GetCoordinatesAsync(toCity);

                if (!fromCoords.HasValue || !toCoords.HasValue)
                {
                    Console.WriteLine($" Не удалось получить координаты для авиа расчета");
                    return 500;
                }

                // Для авиа считаем расстояние по прямой БЕЗ коэффициента дорог
                var straightDistance = CalculateStraightDistance(fromCoords.Value, toCoords.Value);

                Console.WriteLine($" Авиа расстояние по прямой: {straightDistance} км");
                return straightDistance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка авиа расчета: {ex.Message}");
                return 500;
            }
        }

        private async Task<decimal> GetRoadDistanceAsync(string fromCity, string toCity)
        {
            try
            {
                var fromCoords = await GetCoordinatesAsync(fromCity);
                var toCoords = await GetCoordinatesAsync(toCity);

                if (!fromCoords.HasValue || !toCoords.HasValue)
                {
                    Console.WriteLine($"Не удалось получить координаты для наземного расчета");
                    return 500;
                }

                // Для наземного считаем расстояние по прямой и добавляем коэффициент дорог
                var straightDistance = CalculateStraightDistance(fromCoords.Value, toCoords.Value);
                var roadDistance = straightDistance * 1.1m; // коэффициент дорог

                Console.WriteLine($"Наземное расстояние: {roadDistance} км (по прямой: {straightDistance} км)");
                return roadDistance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка наземного расчета: {ex.Message}");
                return 500;
            }
        }

        private async Task<(double lat, double lon)?> GetCoordinatesAsync(string cityName)
        {
            try
            {
                await Task.Delay(1000); // Уважаем лимиты API

                var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(cityName)}&format=json&limit=1";

                Console.WriteLine($" Запрос координат для: {cityName}");

                var response = await _httpClient.GetStringAsync(url);
                var data = JsonSerializer.Deserialize<List<NominatimResponse>>(response);

                if (data?.Count > 0)
                {
                    var result = data[0];
                    Console.WriteLine($"Найден: {result.display_name}");
                    Console.WriteLine($" Координаты: lat={result.lat}, lon={result.lon}");

                    if (double.TryParse(result.lat, NumberStyles.Any, CultureInfo.InvariantCulture, out double lat) &&
                        double.TryParse(result.lon, NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
                    {
                        return (lat, lon);
                    }
                    else
                    {
                        Console.WriteLine($" Ошибка парсинга координат");
                        return null;
                    }
                }

                Console.WriteLine($" Город не найден: {cityName}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка геокодинга {cityName}: {ex.Message}");
                return null;
            }
        }

        private decimal CalculateStraightDistance((double lat, double lon) from, (double lat, double lon) to)
        {
            const double R = 6371; // Радиус Земли в км

            var dLat = ToRadians(to.lat - from.lat);
            var dLon = ToRadians(to.lon - from.lon);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(from.lat)) * Math.Cos(ToRadians(to.lat)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;

            return Math.Round((decimal)distance, 1);
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180;

        private bool IsAirTransport(int transportTypeId)
        {
            // Проверьте какой ID у авиа транспорта в вашей БД
            return transportTypeId == 2; // Настройте под вашу БД
        }
    }

    public class NominatimResponse
    {
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
    }
}
