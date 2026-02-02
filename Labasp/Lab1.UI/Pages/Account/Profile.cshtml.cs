using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

public class ProfileModel : PageModel
{
    public string Email => User.Identity!.Name!;
    public string AvatarUrl => User.Claims.FirstOrDefault(c => c.Type == "avatar")?.Value
                            ?? "/images/default-profile-picture.png";

    [BindProperty]
    public IFormFile? Avatar { get; set; }

    public async Task<IActionResult> OnPost()
    {
        if (Avatar != null && Avatar.Length > 0)
        {
            // 1) Создаём папку, если её нет
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // 2) Генерируем имя файла
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(Avatar.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            // 3) Сохраняем файл
            using (var stream = System.IO.File.Create(filePath))
            {
                await Avatar.CopyToAsync(stream);
            }

            // 4) Формируем URL для UI
            var avatarUrl = $"/avatars/{fileName}";

            // 5) Немедленно обновляем claim в cookie без перелогина
            var identity = (ClaimsIdentity)User.Identity!;
            var oldAvatar = identity.FindFirst("avatar");
            if (oldAvatar != null) identity.RemoveClaim(oldAvatar);
            identity.AddClaim(new Claim("avatar", avatarUrl));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToPage();
        }

        return RedirectToPage();
    }
}
