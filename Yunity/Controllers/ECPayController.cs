using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography.Xml;
using Yunity.DTO;
using Yunity.Models;


namespace Yunity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ECPayController : ControllerBase
    {
        ILogger<ECPayController> _logger;
        BuildingDataContext _context;
        public ECPayController(ILogger<ECPayController> logger, BuildingDataContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("/AddPayInfo/{id}")] 
        public string AddPayInfo([FromForm] IFormCollection PayInfo)
        {
            _logger.LogInformation($"MerchantTradeNo:{PayInfo}, 時間:{DateTime.Now}");
            return "OK";
        }
        [HttpPost("AddAccountInfo")]
        public string AddAccountInfo(AccountInfoDTO AccountInfo)
        {
            _logger.LogInformation($"MerchantTradeNo:{AccountInfo}, 時間:{DateTime.Now}");
            return "OK";
        }
    }
}
