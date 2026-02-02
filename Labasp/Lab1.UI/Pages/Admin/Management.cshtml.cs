using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.UI.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class ManagementModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}