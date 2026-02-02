using Lab1.Domain.Entities;
using Lab1.BlazorWasm.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Lab1.BlazorWasm.Services
{
    public class DataService : IDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string _pageSize;
        private readonly JsonSerializerOptions _serializerOptions;

        public event Action? DataLoaded;

        public List<Category> Categories { get; set; } = new();
        public List<Sweet> Sweets { get; set; } = new();
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1;
        public Category? SelectedCategory { get; set; }
        public bool UseLocalData { get; set; }

        public DataService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _pageSize = configuration.GetSection("ItemsPerPage").Value ?? "3";
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Загружает демонстрационные данные (seed data) локально
        /// </summary>
        public async Task LoadSeedDataAsync()
        {
            try
            {
                // Создаем категории
                var categories = new List<Category>
                {
                    new Category { Id = 1, Name = "Шоколад", NormalizedName = "chocolates" },
                    new Category { Id = 2, Name = "Мармелад", NormalizedName = "marmalade" },
                    new Category { Id = 3, Name = "Зефир", NormalizedName = "marshmallow" },
                    new Category { Id = 4, Name = "Печенье", NormalizedName = "cookies" },
                    new Category { Id = 5, Name = "Конфеты", NormalizedName = "candies" },
                    new Category { Id = 6, Name = "Торты", NormalizedName = "cakes" }
                };

                // Создаем сладости с ссылками на категории
                var sweets = new List<Sweet>
                {
                    new Sweet
                    {
                        Id = 1,
                        Name = "Дубайский шоколад",
                        Description = "Шоколад с фисташковой и кунжутной пастой",
                        Category = categories[0],
                        CategoryId = 1,
                        Price = 15.50m,
                        Image = "/images/dubai-chocolate.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Id = 2,
                        Name = "Мармеладные мишки",
                        Description = "Цветные мармеладные мишки",
                        Category = categories[1],
                        CategoryId = 2,
                        Price = 12.00m,
                        Image = "/images/gummy-bears.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Id = 3,
                        Name = "Зефир ванильный",
                        Description = "Нежный ванильный зефир",
                        Category = categories[2],
                        CategoryId = 3,
                        Price = 18.75m,
                        Image = "/images/vanilla-marshmallow.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Id = 4,
                        Name = "Печенье с шоколадом",
                        Description = "Хрустящее печенье с кусочками шоколада",
                        Category = categories[3],
                        CategoryId = 4,
                        Price = 25.00m,
                        Image = "/images/chocolate-cookies.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Id = 5,
                        Name = "Карамель",
                        Description = "Сладкая сливочная карамель",
                        Category = categories[4],
                        CategoryId = 5,
                        Price = 8.00m,
                        Image = "/images/caramel-candy.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Id = 6,
                        Name = "Шоколадный торт",
                        Description = "Нежный шоколадный торт с кремом",
                        Category = categories[5],
                        CategoryId = 6,
                        Price = 35.00m,
                        Image = "/images/cake.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Id = 7,
                        Name = "Бельгийский шоколад",
                        Description = "Темный шоколад с орехами",
                        Category = categories[0],
                        CategoryId = 1,
                        Price = 22.00m,
                        Image = "/images/belgian-chocolate.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Id = 8,
                        Name = "Фруктовый мармелад",
                        Description = "Мармелад с натуральным соком",
                        Category = categories[1],
                        CategoryId = 2,
                        Price = 14.50m,
                        Image = "/images/fruit-marmalade.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Id = 9,
                        Name = "Малиновый зефир",
                        Description = "Зефир с малиновым вкусом",
                        Category = categories[2],
                        CategoryId = 3,
                        Price = 19.99m,
                        Image = "/images/raspberry-marshmallow.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Id = 10,
                        Name = "Овсяное печенье",
                        Description = "Полезное овсяное печенье с изюмом",
                        Category = categories[3],
                        CategoryId = 4,
                        Price = 18.50m,
                        Image = "/images/oatmeal-cookies.jpg",
                        ContentType = "image/jpeg"
                    }
                };

                Categories = categories;
                Sweets = sweets;
                Success = true;
                ErrorMessage = string.Empty;
                UseLocalData = true;

                // Уведомляем подписчиков о загрузке данных
                DataLoaded?.Invoke();

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Success = false;
                ErrorMessage = $"Ошибка при загрузке демо-данных: {ex.Message}";
                DataLoaded?.Invoke();
            }
        }

        public async Task GetCategoryListAsync()
        {
            try
            {
                // Пробуем получить данные с сервера без токена
                var response = await _httpClient.GetAsync("api/categories");

                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<Category>>(_serializerOptions);
                    Categories = categories ?? new List<Category>();
                    Success = true;
                    ErrorMessage = string.Empty;
                    UseLocalData = false;
                }
                else
                {
                    // Если сервер недоступен или требует авторизации, используем локальные данные
                    Console.WriteLine($"Сервер вернул ошибку: {response.StatusCode}. Используем локальные данные.");
                    await LoadSeedDataAsync();
                }
            }
            catch (Exception ex)
            {
                // Если возникло исключение (сервер недоступен), используем локальные данные
                Console.WriteLine($"Исключение при получении категорий: {ex.Message}. Используем локальные данные.");
                await LoadSeedDataAsync();
            }

            DataLoaded?.Invoke();
        }

        public async Task GetSweetListAsync(int pageNo = 1)
        {
            // Если мы уже используем локальные данные, просто фильтруем их
            if (UseLocalData && Sweets.Any())
            {
                await ApplyLocalPagingAndFiltering(pageNo);
                return;
            }

            try
            {
                var route = new StringBuilder("api/sweets");

                // добавить категорию в маршрут
                if (SelectedCategory is not null)
                {
                    route.Append($"/{SelectedCategory.NormalizedName}");
                }

                // Создаем query string вручную
                var queryParams = new List<string>();

                // добавить номер страницы
                queryParams.Add($"pageNo={pageNo}");

                // добавить размер страницы
                if (!_pageSize.Equals("3"))
                {
                    queryParams.Add($"pageSize={_pageSize}");
                }

                // добавить строку запроса в Url
                if (queryParams.Count > 0)
                {
                    route.Append("?");
                    route.Append(string.Join("&", queryParams));
                }

                var response = await _httpClient.GetAsync(route.ToString());

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Sweet>>>(_serializerOptions);

                    if (result?.Success == true && result.Data != null)
                    {
                        Sweets = result.Data.Items.ToList();
                        TotalPages = result.Data.TotalPages;
                        CurrentPage = result.Data.PageNo;
                        Success = true;
                        ErrorMessage = string.Empty;
                        UseLocalData = false;
                    }
                    else
                    {
                        // Если сервер вернул ошибку в ответе, используем локальные данные
                        Console.WriteLine($"Сервер вернул ошибку в ответе: {result?.ErrorMessage}. Используем локальные данные.");
                        await LoadSeedDataAsync();
                        await ApplyLocalPagingAndFiltering(pageNo);
                    }
                }
                else
                {
                    // Если сервер недоступен или требует авторизации, используем локальные данные
                    Console.WriteLine($"Сервер вернул ошибку: {response.StatusCode}. Используем локальные данные.");
                    await LoadSeedDataAsync();
                    await ApplyLocalPagingAndFiltering(pageNo);
                }
            }
            catch (Exception ex)
            {
                // Если возникло исключение (сервер недоступен), используем локальные данные
                Console.WriteLine($"Исключение при получении сладостей: {ex.Message}. Используем локальные данные.");
                await LoadSeedDataAsync();
                await ApplyLocalPagingAndFiltering(pageNo);
            }

            DataLoaded?.Invoke();
        }

        /// <summary>
        /// Применяет пагинацию и фильтрацию к локальным данным
        /// </summary>
        private async Task ApplyLocalPagingAndFiltering(int pageNo = 1)
        {
            try
            {
                var filteredSweets = Sweets.AsEnumerable();

                // Фильтрация по категории
                if (SelectedCategory is not null)
                {
                    filteredSweets = filteredSweets.Where(s => s.Category?.NormalizedName == SelectedCategory.NormalizedName);
                }

                // Преобразуем в список для пагинации
                var allSweets = filteredSweets.ToList();
                int pageSize = int.TryParse(_pageSize, out var size) ? size : 3;

                // Рассчитываем пагинацию
                int totalItems = allSweets.Count;
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                // Корректируем номер страницы
                CurrentPage = Math.Max(1, Math.Min(pageNo, TotalPages));

                // Получаем элементы для текущей страницы
                int skip = (CurrentPage - 1) * pageSize;
                Sweets = allSweets.Skip(skip).Take(pageSize).ToList();

                Success = true;
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                Success = false;
                ErrorMessage = $"Ошибка при обработке локальных данных: {ex.Message}";
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Загружает товары для конкретной категории
        /// </summary>
        public async Task LoadCategorySweetsAsync(string categoryNormalizedName, int pageNo = 1)
        {
            try
            {
                // Находим категорию
                var category = Categories.FirstOrDefault(c => c.NormalizedName == categoryNormalizedName);

                if (category == null)
                {
                    // Если категория не найдена, сначала загружаем данные
                    if (!Categories.Any())
                    {
                        await GetCategoryListAsync();
                    }

                    category = Categories.FirstOrDefault(c => c.NormalizedName == categoryNormalizedName);
                }

                if (category != null)
                {
                    SelectedCategory = category;

                    // Если используем локальные данные, применяем фильтрацию
                    if (UseLocalData)
                    {
                        await ApplyLocalPagingAndFiltering(pageNo);
                    }
                    else
                    {
                        // Иначе загружаем с сервера
                        await GetSweetListAsync(pageNo);
                    }
                }
                else
                {
                    Success = false;
                    ErrorMessage = "Категория не найдена";
                    DataLoaded?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Success = false;
                ErrorMessage = $"Ошибка при загрузке товаров категории: {ex.Message}";
                DataLoaded?.Invoke();
            }
        }

        /// <summary>
        /// Загружает все данные (категории и товары)
        /// </summary>
        public async Task LoadAllDataAsync()
        {
            await GetCategoryListAsync();

            // Если после загрузки категорий мы перешли на локальные данные,
            // то сразу показываем первую страницу товаров
            if (UseLocalData || !Sweets.Any())
            {
                await GetSweetListAsync(1);
            }
        }

        /// <summary>
        /// Сбрасывает фильтр по категории
        /// </summary>
        public async Task ClearCategoryFilterAsync()
        {
            SelectedCategory = null;
            await GetSweetListAsync(1);
        }

        /// <summary>
        /// Очищает все данные в сервисе
        /// </summary>
        public void ClearData()
        {
            Categories.Clear();
            Sweets.Clear();
            SelectedCategory = null;
            CurrentPage = 1;
            TotalPages = 0;
            Success = false;
            ErrorMessage = string.Empty;
            UseLocalData = false;

            DataLoaded?.Invoke();
        }

        /// <summary>
        /// Переключается между локальными данными и сервером
        /// </summary>
        public async Task ToggleDataModeAsync(bool useLocalData)
        {
            UseLocalData = useLocalData;
            ClearData();

            if (useLocalData)
            {
                await LoadSeedDataAsync();
            }
            else
            {
                await GetCategoryListAsync();
                await GetSweetListAsync(1);
            }
        }
    }
}