using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Lab1.UI.HelperClasses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lab1.UI.Services.Authentication
{
    // Предполагаем, что ITokenAccessor теперь имеет сигнатуру:
    // public Task<string?> GetUserAccessTokenAsync();
    public class KeycloakTokenAccessor : ITokenAccessor
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly KeycloakData _keycloakData;
        private readonly HttpClient _httpClient;
        private readonly ILogger<KeycloakTokenAccessor> _logger;

        public KeycloakTokenAccessor(
            IHttpContextAccessor contextAccessor,
            IOptions<KeycloakData> options,
            HttpClient httpClient,
            ILogger<KeycloakTokenAccessor> logger)
        {
            _contextAccessor = contextAccessor;
            _keycloakData = options.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает актуальный Access Token пользователя, обновляя его при необходимости.
        /// (Новый публичный метод для Delegating Handler)
        /// </summary>
        public async Task<string?> GetUserAccessTokenAsync()
        {
            var context = _contextAccessor.HttpContext;
            if (context == null) return null;

            // 1. Извлекаем токены и время истечения из сессии
            var accessToken = await context.GetTokenAsync("access_token");
            var refreshToken = await context.GetTokenAsync("refresh_token");
            var expiresAtString = await context.GetTokenAsync("expires_at");

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(expiresAtString))
            {
                _logger.LogDebug("One or more tokens (access/refresh/expires_at) not found in session. User likely signed out or session expired.");
                return null;
            }

            // 2. Проверяем, не истек ли токен
            if (!DateTimeOffset.TryParse(expiresAtString, out var expires))
            {
                _logger.LogError("Failed to parse 'expires_at' token value: {ExpiresAtString}", expiresAtString);
                return null;
            }

            // Если токен истекает менее чем через 60 секунд (или уже истек), пытаемся его обновить.
            if (expires < DateTimeOffset.Now.AddSeconds(60))
            {
                _logger.LogInformation("Access Token expired or about to expire. Attempting refresh.");

                // 3. Обновляем токен
                var newTokens = await RefreshTokenAsync(refreshToken);

                if (newTokens.HasValue)
                {
                    _logger.LogInformation("Token successfully refreshed.");
                    // 4. Сохраняем новые токены обратно в сессию
                    await UpdateAuthenticationSession(context, newTokens.Value);
                    return newTokens.Value.access_token;
                }

                // 4.1. Обновление не удалось. Требуется повторный вход.
                _logger.LogError("Token refresh failed. Signing out user.");
                // Не используйте SignOutAsync здесь, так как это может вызвать бесконечный цикл 
                // при обращении к API. Лучше просто вернуть null.
                // await context.SignOutAsync(); 
                return null;
            }

            // Токен еще действителен
            return accessToken;
        }

        /// <summary>
        /// Устанавливает заголовок Authorization для HttpClient (Сохранено для обратной совместимости, но не рекомендуется)
        /// </summary>
        /// <param name="httpClient">HttpClient для запроса</param>
        /// <param name="isClient">Использовать client credentials вместо пользовательского токена</param>
        public async Task SetAuthorizationHeaderAsync(HttpClient httpClient, bool isClient)
        {
            string? token;

            if (isClient)
            {
                // Для клиентских запросов (client credentials)
                token = await GetClientToken();
                _logger.LogDebug("Using Client Access Token.");
            }
            else
            {
                // Для пользовательских запросов: используем новый публичный метод
                token = await GetUserAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("User Access Token is not available or could not be refreshed. Authorization header is not set.");
                    return;
                }
                _logger.LogDebug("Using User Access Token.");
            }

            // Установка заголовка Authorization: Bearer <token>
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Получает токен client credentials для операций от имени клиента
        /// </summary>
        private async Task<string> GetClientToken()
        {
            // ... (Ваш код GetClientToken без изменений)
            var requestUri = $"{_keycloakData.Host}/realms/{_keycloakData.Realm}/protocol/openid-connect/token";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _keycloakData.ClientId),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_secret", _keycloakData.ClientSecret)
            });

            var response = await _httpClient.PostAsync(requestUri, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to get client token: {StatusCode}. Response: {Content}", response.StatusCode, errorContent);
                throw new HttpRequestException($"Failed to get client token: {response.StatusCode}");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonObject = JsonNode.Parse(jsonString);
            return jsonObject?["access_token"]?.GetValue<string>()
                ?? throw new InvalidOperationException("Access token not found in client response");
        }

        // --- Новые приватные методы для обновления токена (без изменений) ---

        /// <summary>
        /// Обновляет токен с помощью Refresh Token через Keycloak.
        /// </summary>
        private async Task<(string access_token, string refresh_token, string expires_at)?> RefreshTokenAsync(string oldRefreshToken)
        {
            var requestUri = $"{_keycloakData.Host}/realms/{_keycloakData.Realm}/protocol/openid-connect/token";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _keycloakData.ClientId),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", oldRefreshToken),
                new KeyValuePair<string, string>("client_secret", _keycloakData.ClientSecret)
            });

            var response = await _httpClient.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonNode.Parse(jsonString);

                var accessToken = jsonObject?["access_token"]?.GetValue<string>();
                var newRefreshToken = jsonObject?["refresh_token"]?.GetValue<string>();
                var expiresIn = jsonObject?["expires_in"]?.GetValue<int>() ?? 0;

                if (accessToken != null && newRefreshToken != null)
                {
                    // Вычисляем новое время истечения
                    var expiresAt = DateTimeOffset.Now.AddSeconds(expiresIn).ToString();
                    return (accessToken, newRefreshToken, expiresAt);
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Keycloak refresh token request failed: {StatusCode}. Response: {Content}", response.StatusCode, errorContent);
            }
            return null;
        }

        /// <summary>
        /// Обновляет токены в сессии аутентификации пользователя.
        /// </summary>
        private async Task UpdateAuthenticationSession(HttpContext context, (string access_token, string refresh_token, string expires_at) newTokens)
        {
            // Получаем текущий результат аутентификации
            var authenticateResult = await context.AuthenticateAsync();
            if (authenticateResult?.Properties != null)
            {
                // Обновляем токены и время истечения
                authenticateResult.Properties.UpdateTokenValue("access_token", newTokens.access_token);
                authenticateResult.Properties.UpdateTokenValue("refresh_token", newTokens.refresh_token);
                authenticateResult.Properties.UpdateTokenValue("expires_at", newTokens.expires_at);

                // Переподписываем пользователя с обновленными токенами
                await context.SignInAsync(authenticateResult.Principal, authenticateResult.Properties);
                _logger.LogDebug("User session tokens updated.");
            }
            else
            {
                _logger.LogWarning("Could not update user session tokens: AuthenticateAsync failed.");
            }
        }
    }
}