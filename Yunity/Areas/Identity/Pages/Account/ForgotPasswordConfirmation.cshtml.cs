#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Yunity.Areas.Identity.Pages.Account
{

    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        
        public void OnGet()
        {
        }
    }
}
