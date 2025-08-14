// 路徑: Services/GeocodingHelper.cs

namespace Yunity.Services
{
    public static class GeocodingHelper
    {
        public static async Task<(decimal Latitude, decimal Longitude)?> GetCoordinatesAsync(string address, IHttpClientFactory httpClientFactory)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            var client = httpClientFactory.CreateClient();
            var requestUrl = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1&accept-language=zh-TW";

            client.DefaultRequestHeaders.UserAgent.ParseAdd("Yunity/1.0 (contact: ineedyunity@gmail.com)");

            var response = await client.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var results = Newtonsoft.Json.Linq.JArray.Parse(jsonString);
                if (results.Count > 0)
                {
                    var firstResult = results[0];
                    decimal lat = firstResult.Value<decimal>("lat");
                    decimal lon = firstResult.Value<decimal>("lon");
                    return (lat, lon);
                }
            }
            return null;
        }
    }
}
