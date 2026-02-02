using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace Lab1.UI.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;
        [BindProperty] public string ConfirmPassword { get; set; } = string.Empty;
        [BindProperty] public IFormFile? Avatar { get; set; }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Avatar != null)
            {
                // 1) сохраняем файл
                var fileName = Guid.NewGuid() + Path.GetExtension(Avatar.FileName);
                var path = Path.Combine("wwwroot/avatars", fileName);

                using (var stream = System.IO.File.Create(path))
                    await Avatar.CopyToAsync(stream);

                // 2) создаём URL
                var avatarUrl = "/avatars/" + fileName;

                // 3) кладём в Session
                HttpContext.Session.SetString("PendingAvatarUrl", avatarUrl);
            }

            // 4) Редирект в Keycloak
            return Redirect("/Login");
        }
    }
}
