using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.UI.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost(string returnUrl = "/")
        {
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = returnUrl
                },
                "Cookies",
                "oidc"   // ✅ только oidc
            );
        }

        public IActionResult OnGet(string returnUrl = "/")
        {
            return OnPost(returnUrl);
        }
    }
}
