
namespace Yunity.Services
{
    public class VendorCoordinatesBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<VendorCoordinatesBackgroundService> _logger;
        // 可根據需求調整更新間隔，這裡設定為每小時一次
        private readonly TimeSpan _interval = TimeSpan.FromHours(1);

        public VendorCoordinatesBackgroundService(IServiceProvider serviceProvider, ILogger<VendorCoordinatesBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        //啟動自動更新
        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    _logger.LogInformation("VendorCoordinatesBackgroundService 啟動。");

        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        _logger.LogInformation("背景更新開始，時間：{time}", DateTimeOffset.Now);
        //        try
        //        {
        //            using (var scope = _serviceProvider.CreateScope())
        //            {
        //                var batchService = scope.ServiceProvider.GetRequiredService<BatchGeocodingService>();
        //                int count = await batchService.UpdateVendorCoordinatesAsync();
        //                _logger.LogInformation("背景更新完成，更新筆數：{count}", count);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "背景更新失敗");
        //        }
        //        await Task.Delay(_interval, stoppingToken);
        //    }

        //    _logger.LogInformation("VendorCoordinatesBackgroundService 已停止。");
        //}


        //關閉自動更新
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("VendorCoordinatesBackgroundService 已啟動，但自動更新功能已關閉。");
            // 不執行任何自動更新作業，直接回傳已完成的 Task
            return Task.CompletedTask;
        }
    }
}
