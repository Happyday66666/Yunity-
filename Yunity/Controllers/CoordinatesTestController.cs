using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Yunity.Controllers
{

    [Route("api/[controller]")]
    public class CoordinatesTestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CoordinatesTestController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/CoordinatesTest/TestCoordinates?address=地址
        [HttpGet("TestCoordinates")]
        public async Task<IActionResult> TestCoordinates(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return BadRequest("請提供地址");
            }

            var client = _httpClientFactory.CreateClient();
            // 建立請求 URL，加入 accept-language 參數，並設定 User-Agent
            var requestUrl = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1&accept-language=zh-TW";

           
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Yunity/1.0 (contact: ineedyunity@gmail.com)");

            var response = await client.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                // 設定 JSON 序列化選項，避免中文轉義成 Unicode escape 序列
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var results = JsonSerializer.Deserialize<JsonElement[]>(jsonString, options);
                if (results != null && results.Length > 0)
                {
                    var firstResult = results[0];
                    if (firstResult.TryGetProperty("lat", out var latElement) &&
                        firstResult.TryGetProperty("lon", out var lonElement))
                    {
                        if (decimal.TryParse(latElement.GetString(), out decimal lat) &&
                            decimal.TryParse(lonElement.GetString(), out decimal lon))
                        {
                            // 回傳結果，同樣設定序列化選項
                            var resultObject = new { Latitude = lat, Longitude = lon };
                            var resultJson = JsonSerializer.Serialize(resultObject, options);
                            return Content(resultJson, "application/json");
                        }
                    }
                }
                return Json(new { message = "無法解析地址" }, options);
            }
            return StatusCode((int)response.StatusCode, "API 呼叫失敗");
        }
    }


}
