using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using Yunity.Models;

namespace Yunity.Controllers
{
  public class SuperController : Controller
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        // 檢查 Session 是否可用
        if (context.HttpContext?.Session == null)
        {
            base.OnActionExecuted(context);
            return;
        }

        // 檢查是否已登入
        bool isLoggedIn = context.HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER) != null;

        if (!isLoggedIn)
        {
            // 未登入時重導向至登入頁
            context.Result = RedirectToAction("Login", "Home");
            return;
        }

        // 設定 ViewBag 的登入狀態
        ViewBag.ISLOGGEDIN = isLoggedIn;

        // 呼叫基底方法
        base.OnActionExecuted(context);

        // 反序列化 JSON 從 Session
        var json = context.HttpContext.Session.GetString(CDictionary.SK_LOINGED_USER);
            var user = !string.IsNullOrEmpty(json)
           ? JsonSerializer.Deserialize<TSystemInfo>(json)
           : null;
            if (user != null)
            {


                // 使用登入資訊
                ViewBag.LOGINED_USER = user.FName;
            }
            else
            {
                ViewBag.LOGINED_USER = "Guest"; // 未登入顯示 "Guest"
            }
        }
    }
}


