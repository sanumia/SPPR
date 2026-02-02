using Lab1.API.Models;
using Lab1.API.Services.FileService;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Lab1.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;

        public AccountController(IFileService fileService, IHttpClientFactory clientFactory, IConfiguration config)
        {
            _fileService = fileService;
            _clientFactory = clientFactory;
            _config = config;
        }

        [HttpPost("avatar/upload")]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не получен");

            string avatarUrl = await _fileService.SaveFileAsync(file);

            return Ok(avatarUrl); // Вернём URL, который UI положит в Session
        }

        [HttpPost("avatar/bind")]
        public async Task<IActionResult> BindAvatar([FromBody] BindAvatarRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.AvatarUrl))
                return BadRequest("Неверные данные");

            // 1) Получаем админ-токен
            var keycloak = _config.GetSection("Keycloak");
            var tokenClient = _clientFactory.CreateClient();

            var tokenResponse = await tokenClient.PostAsync(
                $"{keycloak["Host"]}/realms/master/protocol/openid-connect/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = "admin-cli",
                    ["grant_type"] = "password",
                    ["username"] = keycloak["AdminUser"],
                    ["password"] = keycloak["AdminPass"]
                })
            );
            tokenResponse.EnsureSuccessStatusCode();
            var tokenJson = await tokenResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            string adminToken = tokenJson!["access_token"].ToString();

            // 2) Обновляем профиль пользователя в Keycloak
            var http = _clientFactory.CreateClient();
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            var updateData = new
            {
                attributes = new Dictionary<string, string[]>
                {
                    { "avatar", new[] { request.AvatarUrl } }
                }
            };

            var resp = await http.PutAsJsonAsync(
                $"{keycloak["Host"]}/admin/realms/{keycloak["Realm"]}/users/{request.UserId}",
                updateData);

            resp.EnsureSuccessStatusCode();
            return Ok();
        }

        [HttpPost("register")]
        public async Task<RegisterUserResponse> Register([FromForm] RegisterUserRequest request)
        {
            if (request.Password != request.ConfirmPassword)
                return new RegisterUserResponse { Success = false, Message = "Пароли не совпадают" };

            string avatarUrl = await _fileService.SaveFileAsync(request.Avatar);

            var keycloak = _config.GetSection("Keycloak");
            var tokenClient = _clientFactory.CreateClient();
            var tokenResponse = await tokenClient.PostAsync(
                $"{keycloak["Host"]}/realms/master/protocol/openid-connect/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = "admin-cli",
                    ["grant_type"] = "password",
                    ["username"] = keycloak["AdminUser"],
                    ["password"] = keycloak["AdminPass"]
                })
            );
            tokenResponse.EnsureSuccessStatusCode();
            var tokenJson = await tokenResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            string adminToken = tokenJson!["access_token"].ToString();

            var http = _clientFactory.CreateClient();
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            var newUser = new
            {
                username = request.Email,
                email = request.Email,
                enabled = true,
                attributes = new { avatar = new[] { avatarUrl } },
                credentials = new[]
                {
                    new { type = "password", value = request.Password, temporary = false }
                }
            };

            await http.PostAsJsonAsync($"{keycloak["Host"]}/admin/realms/{keycloak["Realm"]}/users", newUser);

            return new RegisterUserResponse { Success = true, Message = "Пользователь создан" };
        }
    }

    public class BindAvatarRequest
    {
        public string UserId { get; set; }
        public string AvatarUrl { get; set; }
    }
}
