using Yunity.Data;
using Yunity.Models;

namespace Yunity.Services
{
    public class BatchGeocodingService
    {
        private readonly BuildingDataContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public BatchGeocodingService(BuildingDataContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

       

      

        //加入更新時間區間
        public async Task<int> UpdateVendorCoordinatesAsync舊(DateTime startDate, DateTime? endDate = null)
        {
            // 若 endDate 為 null，則使用目前時間作為範圍上限
            DateTime effectiveEndDate = endDate ?? DateTime.Now;

            int updateCount = 0;
            // 篩選出符合指定修改時間範圍、且有地址且在 VendorCoordinates 表中沒有對應 CompanyProfileId 的記錄
            var vendorsToUpdate = _context.CompanyProfiles
                .Where(cp => !string.IsNullOrWhiteSpace(cp.ComAddress) &&
                             cp.ComModifyTime >= startDate &&
                             cp.ComModifyTime <= effectiveEndDate &&
                             !_context.VendorCoordinates.Any(vc => vc.CompanyProfileId == cp.Id))
                .ToList();

            foreach (var vendor in vendorsToUpdate)
            {
                // 呼叫地理編碼 API 取得經緯度
                var coordinates = await GetCoordinatesAsync(vendor.ComAddress);
                if (coordinates != null)
                {
                    // 檢查 VendorCoordinates 中是否已有相同經緯度的記錄
                    bool isDuplicate = _context.VendorCoordinates.Any(vc =>
                        vc.Latitude == coordinates.Value.Latitude &&
                        vc.Longitude == coordinates.Value.Longitude);

                    if (isDuplicate)
                    {
                        // 提醒並跳過此筆資料
                        Console.WriteLine($"發現重複經緯度：{coordinates.Value.Latitude}, {coordinates.Value.Longitude}，地址 {vendor.ComAddress} (CompanyProfile ID={vendor.Id}) 已存在。");
                        continue;
                    }

                    var vendorCoordinates = new VendorCoordinate
                    {
                        CompanyProfileId = vendor.Id,
                        Latitude = coordinates.Value.Latitude,
                        Longitude = coordinates.Value.Longitude
                    };

                    _context.VendorCoordinates.Add(vendorCoordinates);
                    updateCount++;
                }
                else
                {
                    // 記錄無法取得地址經緯度的情況
                    Console.WriteLine($"無法取得地址 {vendor.ComAddress} 的經緯度，CompanyProfile ID={vendor.Id}");
                }
            }
            await _context.SaveChangesAsync();
            return updateCount;
        }



        // 使用 Nominatim 進行地理編碼，注意需加入 User-Agent
        private async Task<(decimal Latitude, decimal Longitude)?> GetCoordinatesAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            var client = _httpClientFactory.CreateClient();
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


        // ------------------- VENDOR (廠商) -------------------
        public async Task<int> UpdateVendorCoordinatesAsync00(DateTime startDate, DateTime? endDate = null)
        {
            DateTime effectiveEndDate = endDate ?? DateTime.Now;
            int updateCount = 0;

            var vendorsToUpdate = _context.CompanyProfiles
                .Where(cp => !string.IsNullOrWhiteSpace(cp.ComAddress) &&
                             cp.ComModifyTime >= startDate &&
                             cp.ComModifyTime <= effectiveEndDate &&
                             !_context.VendorCoordinates.Any(vc => vc.CompanyProfileId == cp.Id))
                .ToList();

            foreach (var vendor in vendorsToUpdate)
            {
                var coordinates = await GeocodingHelper.GetCoordinatesAsync(vendor.ComAddress, _httpClientFactory);
                if (coordinates != null)
                {
                    bool isDuplicate = _context.VendorCoordinates.Any(vc =>
                        vc.Latitude == coordinates.Value.Latitude &&
                        vc.Longitude == coordinates.Value.Longitude);

                    if (isDuplicate)
                    {
                        Console.WriteLine($"重複經緯度: {coordinates.Value.Latitude}, {coordinates.Value.Longitude}, 地址: {vendor.ComAddress}");
                        continue;
                    }

                    var vendorCoordinates = new VendorCoordinate
                    {
                        CompanyProfileId = vendor.Id,
                        Latitude = coordinates.Value.Latitude,
                        Longitude = coordinates.Value.Longitude
                    };

                    _context.VendorCoordinates.Add(vendorCoordinates);
                    updateCount++;
                }
                else
                {
                    Console.WriteLine($"無法取得地址 {vendor.ComAddress} 的經緯度 (CompanyProfile ID={vendor.Id})");
                }
            }

            await _context.SaveChangesAsync();
            return updateCount;
        }

        // ------------------- NEAR STORE (周邊商家) -------------------
        public async Task<int> UpdateNearStoreCoordinatesAsync00(DateTime startDate, DateTime? endDate = null)
        {
            DateTime effectiveEndDate = endDate ?? DateTime.Now;
            int updateCount = 0;

            var storesToUpdate = _context.NearStores
                .Where(ns => !string.IsNullOrWhiteSpace(ns.Addr) &&
                             ns.UpdateTime >= startDate &&
                             ns.UpdateTime <= effectiveEndDate &&
                             !_context.NearStoreCoordinates.Any(nsc => nsc.NearStoreId == ns.Id))
                .ToList();

            foreach (var store in storesToUpdate)
            {
                var coordinates = await GeocodingHelper.GetCoordinatesAsync(store.Addr, _httpClientFactory);
                if (coordinates != null)
                {
                    bool isDuplicate = _context.NearStoreCoordinates.Any(nsc =>
                        nsc.Latitude == coordinates.Value.Latitude &&
                        nsc.Longitude == coordinates.Value.Longitude);

                    if (isDuplicate)
                    {
                        Console.WriteLine($"重複經緯度: {coordinates.Value.Latitude}, {coordinates.Value.Longitude}, 地址: {store.Addr}");
                        continue;
                    }

                    var storeCoordinates = new NearStoreCoordinate
                    {
                        NearStoreId = store.Id,
                        Latitude = coordinates.Value.Latitude,
                        Longitude = coordinates.Value.Longitude
                    };

                    _context.NearStoreCoordinates.Add(storeCoordinates);
                    updateCount++;
                }
                else
                {
                    Console.WriteLine($"無法取得地址 {store.Addr} 的經緯度 (Store ID={store.Id})");
                }
            }

            await _context.SaveChangesAsync();
            return updateCount;
        }



        //new
        public async Task<int> UpdateVendorCoordinatesAsync(DateTime startDate, DateTime? endDate = null)
        {
            DateTime effectiveEndDate = endDate ?? DateTime.Now;
            int updateCount = 0;

            // 根據修改時間篩選要更新的廠商資料（不再過濾空地址）
            var vendorsToUpdate = _context.CompanyProfiles
                .Where(cp => cp.ComModifyTime >= startDate &&
                             cp.ComModifyTime <= effectiveEndDate)
                .ToList();

            foreach (var vendor in vendorsToUpdate)
            {
                var coordinates = await GeocodingHelper.GetCoordinatesAsync(vendor.ComAddress, _httpClientFactory);
                if (coordinates != null)
                {
                    // 嘗試找出是否已有對應的 VendorCoordinates 紀錄
                    var existingRecord = _context.VendorCoordinates.FirstOrDefault(vc => vc.CompanyProfileId == vendor.Id);
                    if (existingRecord != null)
                    {
                        // 更新已有紀錄的經緯度（若有不同時才更新）
                        if (existingRecord.Latitude != coordinates.Value.Latitude || existingRecord.Longitude != coordinates.Value.Longitude)
                        {
                            existingRecord.Latitude = coordinates.Value.Latitude;
                            existingRecord.Longitude = coordinates.Value.Longitude;
                            updateCount++;
                        }
                    }
                    else
                    {
                        // 若無紀錄則新增
                        var vendorCoordinates = new VendorCoordinate
                        {
                            CompanyProfileId = vendor.Id,
                            Latitude = coordinates.Value.Latitude,
                            Longitude = coordinates.Value.Longitude
                        };
                        _context.VendorCoordinates.Add(vendorCoordinates);
                        updateCount++;
                    }
                }
                else
                {
                    Console.WriteLine($"無法取得地址 {vendor.ComAddress} 的經緯度 (CompanyProfile ID={vendor.Id})");
                }
            }

            await _context.SaveChangesAsync();
            return updateCount;
        }

        public async Task<int> UpdateNearStoreCoordinatesAsync(DateTime startDate, DateTime? endDate = null)
        {
            DateTime effectiveEndDate = endDate ?? DateTime.Now;
            int updateCount = 0;

            // 根據修改時間篩選要更新的周邊商家資料
            var storesToUpdate = _context.NearStores
                .Where(ns => ns.UpdateTime >= startDate &&
                             ns.UpdateTime <= effectiveEndDate)
                .ToList();

            foreach (var store in storesToUpdate)
            {
                var coordinates = await GeocodingHelper.GetCoordinatesAsync(store.Addr, _httpClientFactory);
                if (coordinates != null)
                {
                    var existingRecord = _context.NearStoreCoordinates.FirstOrDefault(nsc => nsc.NearStoreId == store.Id);
                    if (existingRecord != null)
                    {
                        if (existingRecord.Latitude != coordinates.Value.Latitude || existingRecord.Longitude != coordinates.Value.Longitude)
                        {
                            existingRecord.Latitude = coordinates.Value.Latitude;
                            existingRecord.Longitude = coordinates.Value.Longitude;
                            updateCount++;
                        }
                    }
                    else
                    {
                        var storeCoordinates = new NearStoreCoordinate
                        {
                            NearStoreId = store.Id,
                            Latitude = coordinates.Value.Latitude,
                            Longitude = coordinates.Value.Longitude
                        };
                        _context.NearStoreCoordinates.Add(storeCoordinates);
                        updateCount++;
                    }
                }
                else
                {
                    Console.WriteLine($"無法取得地址 {store.Addr} 的經緯度 (Store ID={store.Id})");
                }
            }

            await _context.SaveChangesAsync();
            return updateCount;
        }

    }
}


