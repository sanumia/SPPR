using Lab1.API.Interfaces;
using Lab1.API.Models;
using Lab1.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Lab1.UI.Services
{
    public class ApiSweetService : ISweetService
    {
        private readonly HttpClient _httpClient;
        private readonly string _pageSize;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<ApiSweetService> _logger;

        public ApiSweetService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiSweetService> logger)
        {
            _httpClient = httpClient;
            _pageSize = configuration.GetSection("ItemsPerPage").Value ?? "3";
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _logger = logger;
        }

        public async Task<ResponseData<ListModel<Sweet>>> GetSweetListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            var urlString = new StringBuilder($"{_httpClient.BaseAddress!.AbsoluteUri}");

            if (categoryNormalizedName != null)
            {
                urlString.Append($"{categoryNormalizedName}/");
            }

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
                    return await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Sweet>>>(_serializerOptions)
                        ?? ResponseData<ListModel<Sweet>>.Error("Данные не получены");
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка десериализации сладостей: {ex.Message}");
                    return ResponseData<ListModel<Sweet>>.Error($"Ошибка: {ex.Message}");
                }
            }

            _logger.LogError($"Данные сладостей не получены от сервера. Error: {response.StatusCode}");
            return ResponseData<ListModel<Sweet>>.Error($"Данные не получены от сервера. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<Sweet>> GetSweetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{id}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<Sweet>>(_serializerOptions)
                        ?? ResponseData<Sweet>.Error("Данные не получены");
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка десериализации сладости: {ex.Message}");
                    return ResponseData<Sweet>.Error($"Ошибка: {ex.Message}");
                }
            }

            return ResponseData<Sweet>.Error($"Сладость не найдена. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<Sweet>> CreateSweetAsync(Sweet sweet)
        {
            sweet.Image ??= "Images/noimage.jpg";

            var content = new StringContent(JsonSerializer.Serialize(sweet, _serializerOptions),
                Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<Sweet>>(_serializerOptions)
                        ?? ResponseData<Sweet>.Error("Данные не получены");
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка десериализации созданной сладости: {ex.Message}");
                    return ResponseData<Sweet>.Error($"Ошибка: {ex.Message}");
                }
            }

            _logger.LogError($"Сладость не создана. Error: {response.StatusCode}");
            return ResponseData<Sweet>.Error($"Сладость не создана. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<Sweet>> UpdateSweetAsync(int id, Sweet sweet)
        {
            var content = new StringContent(JsonSerializer.Serialize(sweet, _serializerOptions),
                Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{id}", content);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return await response.Content.ReadFromJsonAsync<ResponseData<Sweet>>(_serializerOptions)
                        ?? ResponseData<Sweet>.Error("Данные не получены");
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Ошибка десериализации обновленной сладости: {ex.Message}");
                    return ResponseData<Sweet>.Error($"Ошибка: {ex.Message}");
                }
            }

            return ResponseData<Sweet>.Error($"Сладость не обновлена. Error: {response.StatusCode}");
        }

        public async Task<ResponseData<bool>> DeleteSweetAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{id}");

            if (response.IsSuccessStatusCode)
            {
                return new ResponseData<bool> { Data = true };
            }

            return ResponseData<bool>.Error($"Сладость не удалена. Error: {response.StatusCode}");
        }
    }
}