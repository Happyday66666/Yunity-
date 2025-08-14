using System.Text.Json.Serialization;


namespace Yunity.Models
{
    public class CwaWeatherResponse
    {
        [JsonPropertyName("records")]
        public WeatherRecords Records { get; set; }
    }

    public class WeatherRecords
    {
        [JsonPropertyName("Station")]
        public List<Station> Stations { get; set; }
    }

    public class Station
    {
        [JsonPropertyName("StationName")]
        public string StationName { get; set; }

        [JsonPropertyName("GeoInfo")]
        public GeoInfo GeoInfo { get; set; }

        [JsonPropertyName("WeatherElement")]
        public WeatherElement WeatherElement { get; set; }

        // ✅ 取得 `WGS84` 經緯度 (確保正確讀取 API)
        public double Latitude => GeoInfo?.Coordinates?.FirstOrDefault(c => c.CoordinateName == "WGS84")?.StationLatitude ?? 0;
        public double Longitude => GeoInfo?.Coordinates?.FirstOrDefault(c => c.CoordinateName == "WGS84")?.StationLongitude ?? 0;
        
    }


    public class GeoInfo
    {
        [JsonPropertyName("CountyName")]
        public string CountyName { get; set; } = "未知縣市";

        [JsonPropertyName("TownName")]
        public string TownName { get; set; } = "未知區域";

        [JsonPropertyName("Coordinates")] // ✅ 確保 `GeoInfo` 內有 `Coordinates`
        public List<Coordinate> Coordinates { get; set; } = new List<Coordinate>();
    }

    public class Coordinate
    {
        [JsonPropertyName("CoordinateName")]
        public string CoordinateName { get; set; }

        [JsonPropertyName("StationLatitude")]
        public double StationLatitude { get; set; }

        [JsonPropertyName("StationLongitude")]
        public double StationLongitude { get; set; }
    }

    public class WeatherElement
    {
        [JsonPropertyName("Weather")]
        public string Weather { get; set; }

        [JsonPropertyName("AirTemperature")]
        public double AirTemperature { get; set; }

        [JsonPropertyName("RelativeHumidity")]
        public int RelativeHumidity { get; set; }
    }

    // ✅ 新增 36 小時天氣預報結構 (`F-C0032-001`)
    public class WeatherForecastResponse
    {
        [JsonPropertyName("records")]
        public WeatherForecastRecords Records { get; set; }
    }

    public class WeatherForecastRecords
    {
        [JsonPropertyName("location")]
        public List<WeatherLocation> Location { get; set; }
    }

    public class WeatherLocation
    {
        [JsonPropertyName("locationName")]
        public string LocationName { get; set; }

        [JsonPropertyName("weatherElement")]
        public List<WeatherForecastElement> WeatherElements { get; set; }
    }

    public class WeatherForecastElement
    {
        [JsonPropertyName("elementName")]
        public string ElementName { get; set; }

        [JsonPropertyName("time")]
        public List<ForecastTimePeriod> TimePeriods { get; set; }
    }

    public class ForecastTimePeriod
    {
        [JsonPropertyName("startTime")]
        public string StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public string EndTime { get; set; }

        [JsonPropertyName("parameter")]
        public ForecastParameter Parameter { get; set; }
    }

    public class ForecastParameter
    {
        [JsonPropertyName("parameterName")]
        public string ParameterName { get; set; }  // 天氣描述，例如 "陰短暫雨"

        [JsonPropertyName("parameterValue")]
        public string ParameterValue { get; set; }  // 天氣代碼，例如 "9"
    }
    public class ReverseLocation
    {
        public string County { get; set; }
        public string Town { get; set; }
    }

    public class NominatimResponse
    {
        public AddressInfo Address { get; set; }
    }

    public class AddressInfo
    {
        public string County { get; set; }
        public string State { get; set; }
        public string CityDistrict { get; set; }
        public string Suburb { get; set; }
        public string Town { get; set; }
    }


}



