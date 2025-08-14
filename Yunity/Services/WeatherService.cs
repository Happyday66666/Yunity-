using System.Text.Json;
using Yunity.Models;


namespace Yunity.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "CWA-51E96B35-1644-4995-A96B-F75EDE59FE7E"; // ✅ 請替換你的 API Key
        private readonly string _baseUrl = "https://opendata.cwa.gov.tw/api/v1/rest/datastore";
       
        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // ✅ 取得「即時天氣」(O-A0003-001)
        public async Task<Station?> GetNearestWeatherStationAsync(double lat, double lon)
        {
            string url = $"{_baseUrl}/O-A0003-001?Authorization={_apiKey}&format=JSON";
            
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                var weatherData = JsonSerializer.Deserialize<CwaWeatherResponse>(jsonString, options);

                if (weatherData?.Records?.Stations != null)
                {
                    var nearestStation = weatherData.Records.Stations
                        .Where(s => s.Latitude != 0 && s.Longitude != 0)
                        .OrderBy(s => GetDistance(lat, lon, s.Latitude, s.Longitude))
                        .FirstOrDefault();

                    return nearestStation;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API 錯誤: {ex.Message}");
                return null;
            }
        }

        public async Task<Station?> GetNearestWeatherStationAsync_農事(double lat, double lon)
        {
            // 測試：直接寫死鳳山農試所的資料，不呼叫 API
            return await Task.FromResult(new Station
            {
                StationName = "鳳山農試",
                GeoInfo = new GeoInfo
                {
                    CountyName = "高雄市",
                    TownName = "鳳山區",
                    Coordinates = new List<Coordinate>
            {
                new Coordinate
                {
                    CoordinateName = "WGS84",
                    StationLatitude = 22.646611,
                    StationLongitude = 120.356083
                }
            }
                },
                WeatherElement = new WeatherElement
                {
                    AirTemperature = 27,
                    RelativeHumidity = 100,
                    Weather = "陰陣雨或雷雨"
                }
            });
        }

        //地址去查
        public async Task<ReverseLocation?> ReverseGeocodeToCountyAndTownAsync(double lat, double lon)
        {
            var url = $"https://nominatim.openstreetmap.org/reverse?lat={lat}&lon={lon}&format=json";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "YunityApp");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<NominatimResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result?.Address == null) return null;

            return new ReverseLocation
            {
                County = result.Address.County ?? result.Address.State,
                Town = result.Address.CityDistrict ?? result.Address.Suburb ?? result.Address.Town
            };
        }

        public async Task<Station?> GetNearestWeatherStationByCountyAndTownAsync(double lat, double lon)
        {
            var location = await ReverseGeocodeToCountyAndTownAsync(lat, lon);
            if (location == null) return null;


            string url = $"{_baseUrl}/O-A0003-001?Authorization={_apiKey}&format=JSON";
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                var weatherData = JsonSerializer.Deserialize<CwaWeatherResponse>(jsonString, options);

                if (weatherData?.Records?.Stations != null)
                {
                    // 精準比對縣市 + 區（從 GeoInfo 抓 CountyName / TownName）
                    var exactMatch = weatherData.Records.Stations
                        .Where(s =>
                            s.GeoInfo?.CountyName?.Contains(location.County) == true &&
                            s.GeoInfo?.TownName?.Contains(location.Town) == true)
                        .FirstOrDefault();

                    if (exactMatch != null)
                        return exactMatch;

                    // 次選擇：只比對縣市（仍從 GeoInfo 抓）
                    return weatherData.Records.Stations
                        .Where(s =>
                            s.GeoInfo?.CountyName?.Contains(location.County) == true)
                        .FirstOrDefault();
                }


                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API 錯誤: {ex.Message}");
                return null;
            }
        }





        // ✅ 取得「36 小時天氣預報」(F-C0032-001)
        //public async Task<string> GetWeatherDescriptionAsync(string location)
        //{
        //    string url = $"{_baseUrl}/F-C0032-001?Authorization={_apiKey}&format=JSON";

        //    try
        //    {
        //        var response = await _httpClient.GetAsync(url);
        //        response.EnsureSuccessStatusCode();

        //        var jsonString = await response.Content.ReadAsStringAsync();
        //        var options = new JsonSerializerOptions
        //        {
        //            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //            PropertyNameCaseInsensitive = true
        //        };

        //        var weatherData = JsonSerializer.Deserialize<WeatherForecastResponse>(jsonString, options);

        //        var locationData = weatherData?.Records?.Location
        //            .FirstOrDefault(l => l.LocationName == location);

        //        var wxElement = locationData?.WeatherElements
        //            .FirstOrDefault(e => e.ElementName == "Wx"); // ✅ 取得天氣描述
        //        string description = wxElement?.TimePeriods?.FirstOrDefault()?.Parameter?.ParameterName ?? "無法取得天氣描述";
        //        string iconCode = wxElement?.TimePeriods?.FirstOrDefault()?.Parameter?.ParameterValue ?? "1"; // ✅ 預設為 "1" (晴天)

        //        return (description, iconCode);


        //        //return wxElement?.TimePeriods?.FirstOrDefault()?.Parameter?.ParameterName ?? "無法取得天氣描述";
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"API 錯誤: {ex.Message}");
        //        return "無法取得天氣描述";
        //    }
        //}


        public async Task<(string description, string iconCode)> GetWeatherDescriptionAsync(string location)
        {
            string url = $"{_baseUrl}/F-C0032-001?Authorization={_apiKey}&format=JSON";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                var weatherData = JsonSerializer.Deserialize<WeatherForecastResponse>(jsonString, options);

                var locationData = weatherData?.Records?.Location
                    .FirstOrDefault(l => l.LocationName == location);

                var wxElement = locationData?.WeatherElements
                    .FirstOrDefault(e => e.ElementName == "Wx"); // ✅ 取得天氣描述 (Wx)

                string description = wxElement?.TimePeriods?.FirstOrDefault()?.Parameter?.ParameterName ?? "無法取得天氣描述";
                string iconCode = wxElement?.TimePeriods?.FirstOrDefault()?.Parameter?.ParameterValue ?? "1"; // ✅ 預設為 "1" (晴天)

                return (description, iconCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API 錯誤: {ex.Message}");
                return ("無法取得天氣描述", "1"); // 預設為晴天
            }
        }


        public async Task<int?> GetRainProbabilityAsync(string location)
        {
            string url = $"{_baseUrl}/F-C0032-001?Authorization={_apiKey}&format=JSON";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };

                var weatherData = JsonSerializer.Deserialize<WeatherForecastResponse>(jsonString, options);

                var locationData = weatherData?.Records?.Location
                    .FirstOrDefault(l => l.LocationName == location);

                var popElement = locationData?.WeatherElements
                    .FirstOrDefault(e => e.ElementName == "PoP"); // ✅ 取得降雨機率 (PoP)

                // ✅ 取得最近時段的降雨機率值
                string popValue = popElement?.TimePeriods?.FirstOrDefault()?.Parameter?.ParameterName;

                return int.TryParse(popValue, out int probability) ? probability : (int?)null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API 錯誤: {ex.Message}");
                return null;
            }
        }


        // ✅ 計算兩點之間的距離 (Haversine formula)
        private static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // 地球半徑 (km)
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}



