using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Yunity.DTO;
using Yunity.Models;


namespace Yunity.Controllers
{
    public class ECPayViewController : Controller
    {
        private readonly ILogger<ECPayViewController> _logger;
        BuildingDataContext _context;
        public ECPayViewController(ILogger<ECPayViewController> logger, BuildingDataContext context)
        {
            _logger = logger;
            _context = context;
        }


        //要新增到Ecpayorders的內容
        private async Task<string> AddOrder(ECPayDTO EcPayDto)
        {
            EcpayOrder Orders = new EcpayOrder();
            Orders.MemberId = EcPayDto.MerchantID; //綠界金流的商戶ID
            Orders.MerchantTradeNo = EcPayDto.MerchantTradeNo;//紀錄商戶傳送的交易編號，通常是唯一的辨識碼
            Orders.RtnCode = 0; //未付款
            Orders.RtnMsg = "訂單成功尚未付款";
            Orders.TradeNo = EcPayDto.MerchantID.ToString();//交易編號
            Orders.TradeAmt = EcPayDto.TotalAmount;//紀錄交易金額
            Orders.PaymentDate = Convert.ToDateTime(EcPayDto.MerchantTradeDate);//交易日期
            Orders.PaymentType = EcPayDto.PaymentType;
            Orders.PaymentTypeChargeFee = "0";//手續費
            Orders.TradeDate = EcPayDto.MerchantTradeDate;
            Orders.SimulatePaid = 0;//模擬交易狀態，設置為0，表示尚未支付
            _context.EcpayOrders.Add(Orders);
            await _context.SaveChangesAsync();
            return "OK";
        }

        public async Task<ActionResult> Index(int TotalAmount)
        {

            if (TotalAmount == 0)
            {
                _logger.LogWarning("TotalAmount is 0");
            }

            ECPayDTO Ec = new ECPayDTO
            {
                MerchantID = "3002607", //商戶唯一辨識碼ID
                MerchantTradeNo = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20), //交易編號(隨機生成)
                MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),//交易日期
                PaymentType = "aio",//付款方式
                TotalAmount = TotalAmount,//總金額
                TradeDesc = "無",//商品描述
                ItemName = "管理費繳費",//商品名稱
            };

            await AddOrder(Ec);//新增訂單(將資料傳進Ecpayorders中)

            //準備資料傳送給前端，讓用戶進行付款
            var website = $"https://localhost:7091";
            var order = new Dictionary<string, string>
            {
                { "MerchantID", Ec.MerchantID },//商戶ID
                { "MerchantTradeNo",  Ec.MerchantTradeNo},//交易編號，追蹤每筆交易
                { "MerchantTradeDate",  Ec.MerchantTradeDate},//交易時間
                { "PaymentType",  Ec.PaymentType},//aio綠界金流所有的付款方式
                { "TotalAmount",  Convert.ToString(Ec.TotalAmount)},//總金額
                { "TradeDesc", Ec.TradeDesc },//描述這筆交易
                { "ItemName",  Ec.ItemName},//商品名稱
                { "ExpireDate",  "3"},//過期時間--3小時
                { "ReturnURL",  $"{website}/api/ECPay/AddPayInfo"},//支付完成後倒回頁面(商家)
                { "OrderResultURL", $"{website}/ECPayView/PayInfo/{Ec.MerchantTradeNo}"},//付款結果傳回頁面
                { "ChoosePayment", "ALL"},//所有付款方式
                { "PaymentInfoURL",  $"{website}/api/ECPay/AddAccountInfo"},//付款後的帳戶資訊網址
                { "EncryptType",  "1"},//加密
                { "ClientRedirectURL",  $"{website}/ECPayView/AccountInfo/{Ec.MerchantTradeNo}"},//付款完成後，用戶會被導回這個頁面(用戶端)
            };
            //檢查碼
            order["CheckMacValue"] = GetCheckMacValue(order);
            return View(order);
        }

        //如果是Web API Controller, 且需要停用CORS
        //[HttpPost("PayInfo/{id}"]
        //public async Task<ActionResult> PayInfo([FromForm]IFormCollection col)



        //將支付結果更新到資料庫
        [HttpPost]
        public async Task<ActionResult> PayInfo(IFormCollection col)
        {

            var merchantTradeNo = col["MerchantTradeNo"].ToString();

            // 確保 PaymentCode 存在並且是有效的整數
            var paymentCodeStr = col["PaymentCode"].ToString();
            int? paymentCode = null;
            if (!string.IsNullOrEmpty(paymentCodeStr) && int.TryParse(paymentCodeStr, out var parsedPaymentCode))
            {
                paymentCode = parsedPaymentCode;
            }

            // 根據 MerchantTradeNo 找到相應的訂單
            var order = _context.EcpayOrders
                .FirstOrDefault(m => m.MerchantTradeNo == merchantTradeNo);

            if (order != null)
            {
                order.RtnCode = int.Parse(col["RtnCode"]);
                order.RtnMsg = col["RtnMsg"] == "Succeeded" ? "已付款" : "付款失敗";
                order.PaymentDate = Convert.ToDateTime(col["PaymentDate"]);
                order.SimulatePaid = int.Parse(col["SimulatePaid"]);

                // 更新 ManageFee 狀態為已繳費，並保存付款時間和付款方式
                var manageFeesToUpdate = _context.ManageFees
                    .Where(fee => fee.MerchantTradeNo == null && fee.State == 0)  // 檢查未繳費的記錄
                    .ToList();

                foreach (var fee in manageFeesToUpdate)
                {
                    fee.MerchantTradeNo = order.MerchantTradeNo; // 更新 MerchantTradeNo
                    fee.State = 1;  // 標記為已繳費
                    fee.PayType = 1;
                    fee.PayTime = order.PaymentDate; // 設置付款時間
                }

                await _context.SaveChangesAsync(); // 保存 ManageFee 更新
                await _context.SaveChangesAsync(); // 保存 EcpayOrders 訂單變更
            }

            // 收集回應中的資料並回傳給前端
            //var data = new Dictionary<string, string>();
            //foreach (string key in col.Keys)
            //{
            //    data.Add(key, col[key]);
            //}

            //return View("ECpayView", data);

            return RedirectToAction( "List", "UManagementFee");









            //可以用
            //var merchantTradeNo = col["MerchantTradeNo"].ToString();

            //// 確保 PaymentCode 存在並且是有效的整數
            //var paymentCodeStr = col["PaymentCode"].ToString();
            //int? paymentCode = null;
            //if (!string.IsNullOrEmpty(paymentCodeStr) && int.TryParse(paymentCodeStr, out var parsedPaymentCode))
            //{
            //    paymentCode = parsedPaymentCode;
            //}

            //// 根據 MerchantTradeNo 找到相應的訂單
            //var order = _context.EcpayOrders
            //    .FirstOrDefault(m => m.MerchantTradeNo == merchantTradeNo);

            //if (order != null)
            //{
            //    order.RtnCode = int.Parse(col["RtnCode"]);
            //    order.RtnMsg = col["RtnMsg"] == "Succeeded" ? "已付款" : "付款失敗";
            //    order.PaymentDate = Convert.ToDateTime(col["PaymentDate"]);
            //    order.SimulatePaid = int.Parse(col["SimulatePaid"]);

            //    // 更新 ManageFee 狀態為已繳費
            //    var manageFeesToUpdate = _context.ManageFees
            //        .Where(fee => fee.MerchantTradeNo == null && fee.State == 0)  // 檢查未繳費的記錄
            //        .ToList();

            //    foreach (var fee in manageFeesToUpdate)
            //    {
            //        fee.MerchantTradeNo = order.MerchantTradeNo; // 更新 MerchantTradeNo
            //        fee.State = 1;  // 標記為已繳費
            //    }

            //    await _context.SaveChangesAsync(); // 保存 ManageFee 更新
            //    await _context.SaveChangesAsync(); // 保存 EcpayOrders 訂單變更
            //}

            //// 收集回應中的資料並回傳給前端
            //var data = new Dictionary<string, string>();
            //foreach (string key in col.Keys)
            //{
            //    data.Add(key, col[key]);
            //}

            //return View("ECpayView", data);

            //原來的

            //var data = new Dictionary<string, string>();
            //foreach (string key in col.Keys)
            //{
            //    data.Add(key, col[key]);
            //}
            //var Orders = _context.EcpayOrders.ToList().Where(m => m.MerchantTradeNo == col["MerchantTradeNo"]).FirstOrDefault();
            //Orders.RtnCode = int.Parse(col["RtnCode"]);
            //if (col["RtnMsg"] == "Succeeded")
            //{
            //    Orders.RtnMsg = "已付款";
            //    Orders.PaymentDate = Convert.ToDateTime(col["PaymentDate"]);
            //    Orders.SimulatePaid = int.Parse(col["SimulatePaid"]);

            //    await _context.SaveChangesAsync();
            //}
            //return View("ECpayView", data);
        }
        
    
        /// step5 : 取得虛擬帳號 資訊  ClientRedirectURL
        [HttpPost]
        public async Task<ActionResult> AccountInfo(IFormCollection col)
        {
            var data = new Dictionary<string, string>();
            foreach (string key in col.Keys)
            {
                data.Add(key, col[key]);
            }
            var Orders = _context.EcpayOrders.ToList().Where(m => m.MerchantTradeNo == col["MerchantTradeNo"]).FirstOrDefault();
            Orders.RtnCode = int.Parse(col["RtnCode"]);
            if (col["RtnMsg"] == "Succeeded")
            {
                Orders.RtnMsg = "已付款";
                Orders.PaymentDate = Convert.ToDateTime(col["PaymentDate"]);
                Orders.SimulatePaid = int.Parse(col["SimulatePaid"]);
                await _context.SaveChangesAsync();
            }
            return View("ECpayView", data);
        }
        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
            string checkValue = string.Join("&", param);
            //測試用的 HashKey
            var hashKey = "pwFHCqoQZGmho4w6";
            //測試用的 HashIV
            var HashIV = "EkRm7iFT261dpevs";
            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";
            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
            checkValue = GetSHA256(checkValue);
            return checkValue.ToUpper();
        }
        private string GetSHA256(string value)
        {
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            return result.ToString();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
