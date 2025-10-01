using Lab1.API.Interfaces;
using Lab1.API.Models;
using Lab1.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Lab1.UI.Services
{
    public class ApiCategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _pageSize;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<ApiCategoryService> _logger;

        public ApiCategoryService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiCategoryService> logger)
        {
            _httpClient = httpClient;
            _pageSize = configuration.GetSection("ItemsPerPage").Value ?? "3";
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
        }

        public async Task<ResponseData<ListModel<Category>>> GetCategoryListAsync(int pageNo = 1)
        {
            var urlString = new StringBuilder($"{_httpClient.BaseAddress!.AbsoluteUri}");

            if (pageNo > 1)
            {
                urlString.Append($"page{pageNo}");
            }

            if (!_pageSize.Equals("3"))
            {
                urlString.Append(QueryString.Create("pageSize", _pageSize));
            }

            var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Category>>>(_serializerOptions)
                        ?? ResponseData<ListModel<Category>>.Error("Данные не получены");
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка десериализации категорий: {ex.Message}");
                    return ResponseData<ListModel<Category>>.Error($"Ошибка: {ex.Message}");
                }
            }

            _logger.LogError($"Данные категорий не получены от сервера. Error: {response.StatusCode}");
            return ResponseData<ListModel<Category>>.Error($"Данные не получены от сервера. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<Category>> GetCategoryByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{id}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<Category>>(_serializerOptions)
                        ?? ResponseData<Category>.Error("Данные не получены");
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка десериализации категории: {ex.Message}");
                    return ResponseData<Category>.Error($"Ошибка: {ex.Message}");
                }
            }

            return ResponseData<Category>.Error($"Категория не найдена. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<Category>> CreateCategoryAsync(Category category)
        {
            var content = new StringContent(JsonSerializer.Serialize(category, _serializerOptions),
                Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<Category>>(_serializerOptions)
                        ?? ResponseData<Category>.Error("Данные не получены");
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка десериализации созданной категории: {ex.Message}");
                    return ResponseData<Category>.Error($"Ошибка: {ex.Message}");
                }
            }

            _logger.LogError($"Категория не создана. Error: {response.StatusCode}");
            return ResponseData<Category>.Error($"Категория не создана. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<Category>> UpdateCategoryAsync(int id, Category category)
        {
            var content = new StringContent(JsonSerializer.Serialize(category, _serializerOptions),
                Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{id}", content);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<Category>>(_serializerOptions)
                        ?? ResponseData<Category>.Error("Данные не получены");
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка десериализации обновленной категории: {ex.Message}");
                    return ResponseData<Category>.Error($"Ошибка: {ex.Message}");
                }
            }

            return ResponseData<Category>.Error($"Категория не обновлена. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<bool>> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{id}");

            if (response.IsSuccessStatusCode)
            {
                return new ResponseData<bool> { Data = true };
            }

            return ResponseData<bool>.Error($"Категория не удалена. Error: {response.StatusCode}");
        }
    }
}