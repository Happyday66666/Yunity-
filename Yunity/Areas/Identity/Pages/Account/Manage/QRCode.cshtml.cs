using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System.Drawing;
using Yunity.Areas.Identity.Data;
using Yunity.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using BuildingDataContext = Yunity.Models.BuildingDataContext;


namespace Yunity.Areas.Identity.Pages.Account.Manage
{
    public class QRCodeModel : PageModel
    {

        private readonly UserManager<YunityUser> _userManager;
        private readonly SignInManager<YunityUser> _signInManager;
        private readonly BuildingDataContext _buildingDataContext;
        private readonly IWebHostEnvironment _environment;  // 注入 IWebHostEnvironment
        public string QRCodeBase64 { get; set; }
        public string? QRCodeImageBase64 { get; set; }

        public QRCodeModel(
           UserManager<YunityUser> userManager,
           SignInManager<YunityUser> signInManager, BuildingDataContext buildingDataContext, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _buildingDataContext = buildingDataContext;
            _environment = environment;  
        }
       
        //AJAX
        [HttpPost]
        public async Task<IActionResult> OnPostGenerateQRCodeAsyncICON()
        {
            var iconPath = Path.Combine(_environment.WebRootPath, "images", "logos", "YUNITY_icon_cornflowerblue.png");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // 若未登入，回傳 401 狀態碼與錯誤訊息
                return new JsonResult(new { error = "請先登入" }) { StatusCode = 401 };
            }

            var userInfo = await _buildingDataContext.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
            if (userInfo == null)
            {
                // 若找不到用戶資訊，回傳 404 狀態碼與錯誤訊息
                return new JsonResult(new { error = "找不到用戶資訊" }) { StatusCode = 404 };
            }

            string memberInfo = $"{userInfo.FAspUserId}";

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(memberInfo, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);
                int iconSize = 100;

                using (Image icon = Image.FromFile(iconPath))
                {
                    // 取得 logo 圖片並調整大小
                    Bitmap iconBitmap = new Bitmap(icon, new Size(iconSize, iconSize));
                    iconBitmap.MakeTransparent();

                    // 生成 QR Code 圖片，並在中間疊加 logo
                    using (var ms = new MemoryStream(qrCodeBytes))
                    {
                        // 先從 MemoryStream 建立原始 QR Code 圖片
                        Bitmap originalQr = new Bitmap(ms);
                        // 建立一個新的 Bitmap 來存放最終結果（格式使用 32bppArgb）
                        using (Bitmap qrImage = new Bitmap(originalQr.Width, originalQr.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                        {
                            using (Graphics g = Graphics.FromImage(qrImage))
                            {
                                // 清空背景並繪製原始 QR Code
                                g.Clear(Color.White);
                                // 由於 ms 內的資料已讀取完，這裡用原始 QR Code 來繪圖
                                g.DrawImage(originalQr, new Point(0, 0));

                                // 計算 logo 放置在 QR Code 中心的位置
                                int x = (qrImage.Width - iconBitmap.Width) / 2;
                                int y = (qrImage.Height - iconBitmap.Height) / 2;
                                g.DrawImage(iconBitmap, x, y);
                            }

                            // 將最終圖片存回 MemoryStream
                            using (var updatedMs = new MemoryStream())
                            {
                                qrImage.Save(updatedMs, System.Drawing.Imaging.ImageFormat.Png);
                                qrCodeBytes = updatedMs.ToArray();
                            }
                        }
                    }
                }

                // 將生成的 QR Code 存入資料庫（若需要）
                userInfo.QrCord = qrCodeBytes;
                await _buildingDataContext.SaveChangesAsync();

                // 轉換為 Base64 字串後回傳 JSON 結果
                string base64String = Convert.ToBase64String(userInfo.QrCord);
                return new JsonResult(new { qrCodeBase64 = base64String });
            }
        }


    }
}
