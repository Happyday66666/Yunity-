using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using Yunity.Data;
using Yunity.Models;
using Microsoft.AspNetCore.Identity;
using Yunity.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Yunity.Partials;
using BuildingDataContext = Yunity.Models.BuildingDataContext;

namespace Yunity.Controllers
{
    [Route("QRCode")]
    public class QRCodeController : Controller
{
        private readonly BuildingDataContext _context;
        private readonly UserManager<YunityUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        public QRCodeController(BuildingDataContext context, UserManager<YunityUser> userManager,IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }


       
            

        // 生成会员QR码
        // 使用 POST 方法
        [HttpPost("GenerateQRCodeAsync")]
        public async Task<IActionResult> GenerateQRCodeAsync(int memberId)
        {
            // 取得當前登入的用戶資料
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();  // 若用戶未登入，重定向到登入頁面
            }

            // 取得該用戶的 TUserInfo 資料，並根據 FAspUserId 取得對應的大樓 ID
            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
           
            if (userInfo == null)
            {
                return NotFound();
            }

            string memberInfo = $"ID: {userInfo.FAspUserId}";

            //// 创建QR码->存64位元，過長
            //using (var qrGenerator = new QRCodeGenerator())
            //{
            //    var qrCodeData = qrGenerator.CreateQrCode(memberInfo, QRCodeGenerator.ECCLevel.Q);
            //    var qrCode = new PngByteQRCode(qrCodeData);
            //    byte[] qrCodeBytes = qrCode.GetGraphic(20); // 生成 QR 码 PNG 图像的 byte[]

            //    // **转换为 Base64 字符串**
            //    string base64String = Convert.ToBase64String(qrCodeBytes);

            //    // **存入数据库**
            //    userInfo.QrCord = base64String;
            //    await _context.SaveChangesAsync();

            //    // 傳遞 Base64 字符串到 View
            //    ViewData["QRCodeBase64"] = base64String;

            //    return Ok(new { message = "QR 码已存入数据库", qrCodeBase64 = base64String });
            //}

            //二進位
            using (var qrGenerator = new QRCodeGenerator())
            {
                // 創建 QR Code
                var qrCodeData = qrGenerator.CreateQrCode(memberInfo, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);

                // 獲取 QR Code 圖像的二進位資料
                byte[] qrCodeBytes = qrCode.GetGraphic(20);  // 生成較高解析度的 QR Code 圖像

                // 儲存二進位資料到資料庫
                userInfo.QrCord = qrCodeBytes;  // 假設 userInfo.QrCord 欄位的類型為 varbinary(max)
                //_context.SaveChanges();
                // 异步保存数据库更改
                await _context.SaveChangesAsync();
                // 将二进制数据转换为 Base64 字符串
                string base64String = Convert.ToBase64String(userInfo.QrCord);

                // 将 Base64 字符串传递给视图
                ViewData["QRCodeBase64"] = base64String;
               return View("GenerateQRCodeAsync");
                // 返回成功消息
                // return Ok(new { message = "QRCord已成功存入資料庫" });
                
            }
        }


        // 生成会员QR码
        // 使用 POST 方法
        [HttpPost("GenerateQRCodeAsyncICON")]
        public async Task<IActionResult> GenerateQRCodeAsyncICON(int memberId)
        {
            // 取得當前登入的用戶資料
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();  // 若用戶未登入，重定向到登入頁面
            }

            // 取得該用戶的 TUserInfo 資料，並根據 FAspUserId 取得對應的大樓 ID
            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);

            if (userInfo == null)
            {
                return NotFound();
            }

            string memberInfo = $"ID: {userInfo.FAspUserId}";

          
            // 获取图标路径
            string iconPath = Path.Combine(_environment.WebRootPath, "images", "logos", "YUNITY_icon_cornflowerblue.png");

            // 二进制处理，生成 QR Code
            using (var qrGenerator = new QRCodeGenerator())
            {
                // 创建 QR Code 数据
                var qrCodeData = qrGenerator.CreateQrCode(memberInfo, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);

                // 生成较高解析度的 QR Code 图像的二进制数据
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                // 设置图标大小（例如，图标为二维码宽度的1/5）
                int iconSize = 100;  // 你可以根据需要调整这个值，确保它适合你的二维码

                // 生成图标并嵌入
                using (Image icon = Image.FromFile(iconPath))
                {
                    // 确保图标是一个Bitmap并转换为32位ARGB格式
                    Bitmap iconBitmap = new Bitmap(icon);
                    iconBitmap = new Bitmap(iconBitmap, new Size(iconSize, iconSize));

                    // 将图标转换为透明背景，防止色彩冲突
                    iconBitmap.MakeTransparent();

                    // 创建二维码图像
                    using (var ms = new MemoryStream(qrCodeBytes))
                    {
                        // 先将二维码图像转换为一个支持Graphics操作的格式（32bpp ARGB）
                        Bitmap qrImage = new Bitmap(ms);
                        qrImage = new Bitmap(qrImage.Width, qrImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        // 将原来的二维码内容绘制到新创建的32bpp ARGB图像中
                        using (Graphics g = Graphics.FromImage(qrImage))
                        {
                            g.Clear(Color.White);  // 清除背景色
                            g.DrawImage(new Bitmap(ms), new Point(0, 0));  // 绘制原二维码图像

                            // 使用Graphics从qrImage绘制图标
                            int x = (qrImage.Width - iconBitmap.Width) / 2;
                            int y = (qrImage.Height - iconBitmap.Height) / 2;
                            g.DrawImage(iconBitmap, x, y);
                        }

                        // 将处理过的图像保存回二进制数据
                        using (var updatedMs = new MemoryStream())
                        {
                            qrImage.Save(updatedMs, System.Drawing.Imaging.ImageFormat.Png);
                            qrCodeBytes = updatedMs.ToArray();
                        }
                    }
                }

                // 保存二进制资料到数据库
                userInfo.QrCord = qrCodeBytes;  // 假设 userInfo.QrCord 为 varbinary(max) 类型
                await _context.SaveChangesAsync();

                // 将二进制数据转换为 Base64 字符串
                string base64String = Convert.ToBase64String(userInfo.QrCord);

                // 将 Base64 字符串传递给视图
                ViewData["QRCodeBase64"] = base64String;

                // 返回视图
                return View("GenerateQRCodeAsyncICON");
                //return View();
            }
           
        }


        



        public async Task<IActionResult> GetQRCodeAsync(int memberId)
        {
            // 取得當前登入的用戶資料
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();  // 若用戶未登入，重定向到登入頁面
            }

            // 取得該用戶的 TUserInfo 資料，並根據 FAspUserId 取得對應的大樓 ID
            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);

            //if (userInfo == null || string.IsNullOrEmpty(userInfo.QrCord))
            //{
            //    return NotFound("QR 碼不存在");
            //}

            if (userInfo == null || userInfo.QrCord == null || userInfo.QrCord.Length == 0)
            {
                // 输出一些调试信息来检查是否成功获取了数据
                Console.WriteLine("QrCord 数据为空或无效");
                return NotFound("QR 碼不存在");
            }

            // 将二进制数据转换为 Base64 字符串
            string base64String = Convert.ToBase64String(userInfo.QrCord);

            // 将 Base64 字符串传递给视图
            ViewData["QRCodeBase64"] = base64String;


            // **将 Base64 还原为 byte[] 并返回图片**
            //byte[] qrCodeBytes = Convert.FromBase64String(userInfo.QrCord);
            //return File(userInfo.QrCord, "image/png");
            return View();
        }


        //TestNow
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

            var userInfo = await _context.TusersInfos
                .FirstOrDefaultAsync(u => u.FAspUserId == user.Id);
            if (userInfo == null)
            {
                // 若找不到用戶資訊，回傳 404 狀態碼與錯誤訊息
                return new JsonResult(new { error = "找不到用戶資訊" }) { StatusCode = 404 };
            }

            string memberInfo = $"ID: {userInfo.FAspUserId}";

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
                        // 從 MemoryStream 建立原始 QR Code 圖片
                        Bitmap originalQr = new Bitmap(ms);
                        // 建立一個新的 Bitmap 存放最終結果（格式 32bppArgb）
                        using (Bitmap qrImage = new Bitmap(originalQr.Width, originalQr.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                        {
                            using (Graphics g = Graphics.FromImage(qrImage))
                            {
                                // 清空背景並繪製原始 QR Code
                                g.Clear(Color.White);
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
                await _context.SaveChangesAsync();

                // 將二進位資料轉成 Base64 字串回傳
                string base64String = Convert.ToBase64String(userInfo.QrCord);
                return new JsonResult(new { qrCodeBase64 = base64String });
            }
        }


    }
}
