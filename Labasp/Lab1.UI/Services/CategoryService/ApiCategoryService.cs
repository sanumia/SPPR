using Lab1.Domain.Entities;
using Lab1.Domain.Models;
using Lab1.UI.Services.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Lab1.UI.Services.CategoryService
{
    public class ApiCategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenAccessor _tokenAccessor;
        private readonly ILogger<ApiCategoryService> _logger;
        private readonly IMemoryCache _cache;
        private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

        public ApiCategoryService(
            HttpClient httpClient,
            ITokenAccessor tokenAccessor,
            ILogger<ApiCategoryService> logger,
            IMemoryCache cache)
        {
            _httpClient = httpClient;
            _tokenAccessor = tokenAccessor;
            _logger = logger;
            _cache = cache;
        }

        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            try
            {
                const string cacheKey = "categories_all";

                if (_cache.TryGetValue(cacheKey, out ResponseData<List<Category>> cachedData))
                    return cachedData;

                _logger.LogInformation("Requesting categories from API");

                // Добавляем токен перед GET-запросом
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);

                var list = await _httpClient.GetFromJsonAsync<List<Category>>(string.Empty, _serializerOptions);
                var response = list != null
                    ? ResponseData<List<Category>>.Success(list)
                    : ResponseData<List<Category>>.Error("No data");

                _cache.Set(cacheKey, response, TimeSpan.FromSeconds(60));
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error getting category list");
                return ResponseData<List<Category>>.Error($"Request failed: {ex.Message}");
            }
        }

        public async Task<ResponseData<Category>> GetCategoryByIdAsync(int id)
        {
            try
            {
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);
                var url = $"{id}";
                var data = await _httpClient.GetFromJsonAsync<Category>(url, _serializerOptions);
                return data != null
                    ? ResponseData<Category>.Success(data)
                    : ResponseData<Category>.Error("Not found");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error getting category by id {Id}", id);
                return ResponseData<Category>.Error($"Request failed: {ex.Message}");
            }
        }
        public async Task<ResponseData<Category>> CreateCategoryAsync(Category category)
        {
            try
            {
                // Для операций создания используем токен пользователя
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);

                var response = await _httpClient.PostAsJsonAsync(string.Empty, category, _serializerOptions);
                if (response.IsSuccessStatusCode)
                {
                    var created = await response.Content.ReadFromJsonAsync<Category>(_serializerOptions);
                    return created != null
                        ? ResponseData<Category>.Success(created)
                        : ResponseData<Category>.Error("Empty response");
                }
                return ResponseData<Category>.Error($"Create failed: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return ResponseData<Category>.Error($"Объект не добавлен. Error: {ex.Message}");
            }
        }

        public async Task<ResponseData<Category>> UpdateCategoryAsync(Category category)
        {
            try
            {
                // Для операций обновления используем токен пользователя
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);

                var response = await _httpClient.PutAsJsonAsync($"{category.Id}", category, _serializerOptions);
                if (response.IsSuccessStatusCode)
                {
                    return ResponseData<Category>.Success(category);
                }
                return ResponseData<Category>.Error($"Update failed: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {Id}", category.Id);
                return ResponseData<Category>.Error($"Объект не обновлен. Error: {ex.Message}");
            }
        }

        public async Task<ResponseData<bool>> DeleteCategoryAsync(int id)
        {
            try
            {
                // Для операций удаления используем токен пользователя
                await _tokenAccessor.SetAuthorizationHeaderAsync(_httpClient, false);

                var response = await _httpClient.DeleteAsync($"{id}");
                return response.IsSuccessStatusCode
                    ? ResponseData<bool>.Success(true)
                    : ResponseData<bool>.Error($"Delete failed: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {Id}", id);
                return ResponseData<bool>.Error($"Объект не удален. Error: {ex.Message}");
            }
        }
    }
}