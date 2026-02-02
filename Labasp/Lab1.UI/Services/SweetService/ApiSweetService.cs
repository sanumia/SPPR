using Lab1.Domain.Entities;
using Lab1.Domain.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Lab1.UI.Services.SweetService
{
    public class ApiSweetService : ISweetService
    {
        private readonly HttpClient _authenticatedClient;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly List<Category> _categories;
        private readonly List<Sweet> _sweets;

        // Конструктор с HttpClient для DI
        public ApiSweetService(HttpClient authenticatedClient, IHttpClientFactory httpClientFactory)
        {
            _authenticatedClient = authenticatedClient;
            _httpClientFactory = httpClientFactory;

            // Инициализация локальных данных
            _categories = SetupCategories();
            _sweets = SetupSweets(_categories);
        }

        // ============================================================
        // GET LIST (каталог / анонимный)
        // ============================================================
        public Task<ResponseData<ListModel<Sweet>>> GetSweetListAsync(string categoryNormalizedName = null, int pageNo = 1)
        {
            var sweets = string.IsNullOrEmpty(categoryNormalizedName)
                ? _sweets
                : _sweets.Where(s => s.Category.NormalizedName == categoryNormalizedName).ToList();

            int pageSize = 10;
            var paged = sweets.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

            var result = new ListModel<Sweet>
            {
                Items = paged,
                TotalCount = sweets.Count
            };

            return Task.FromResult(ResponseData<ListModel<Sweet>>.Success(result));
        }

        // ============================================================
        // GET BY ID
        // ============================================================
        public Task<ResponseData<Sweet>> GetSweetByIdAsync(int id)
        {
            var sweet = _sweets.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(
                sweet != null
                    ? ResponseData<Sweet>.Success(sweet)
                    : ResponseData<Sweet>.Error("Not found")
            );
        }

        // ============================================================
        // CREATE
        // ============================================================
        public Task<ResponseData<Sweet>> CreateSweetAsync(Sweet sweet, IFormFile? image = null)
        {
            sweet.Id = _sweets.Max(s => s.Id) + 1;

            if (sweet.Category != null)
                sweet.Category = _categories.FirstOrDefault(c => c.Id == sweet.Category.Id);

            _sweets.Add(sweet);

            return Task.FromResult(ResponseData<Sweet>.Success(sweet));
        }

        // ============================================================
        // UPDATE
        // ============================================================
        public Task<ResponseData<Sweet>> UpdateSweetAsync(Sweet sweet, IFormFile? image = null)
        {
            var existing = _sweets.FirstOrDefault(s => s.Id == sweet.Id);
            if (existing == null)
                return Task.FromResult(ResponseData<Sweet>.Error("Not found"));

            existing.Name = sweet.Name;
            existing.Description = sweet.Description;
            existing.Price = sweet.Price;
            if (sweet.Category != null)
                existing.Category = _categories.FirstOrDefault(c => c.Id == sweet.Category.Id);

            return Task.FromResult(ResponseData<Sweet>.Success(existing));
        }

        // ============================================================
        // DELETE
        // ============================================================
        public Task<ResponseData<bool>> DeleteSweetAsync(int id)
        {
            var sweet = _sweets.FirstOrDefault(s => s.Id == id);
            if (sweet == null)
                return Task.FromResult(ResponseData<bool>.Error("Not found"));

            _sweets.Remove(sweet);
            return Task.FromResult(ResponseData<bool>.Success(true));
        }

        // ============================================================
        // Setup initial categories
        // ============================================================
        private List<Category> SetupCategories()
        {
            return new List<Category>
            {
                new Category { Id = 1, Name = "Шоколад", NormalizedName = "chocolates" },
                new Category { Id = 2, Name = "Мармелад", NormalizedName = "marmalade" },
                new Category { Id = 3, Name = "Зефир", NormalizedName = "marshmallow" },
                new Category { Id = 4, Name = "Печенье", NormalizedName = "cookies" },
                new Category { Id = 5, Name = "Карамель", NormalizedName = "candies" },
                new Category { Id = 6, Name = "Торт", NormalizedName = "cakes" }
            };
        }

        // ============================================================
        // Setup initial sweets
        // ============================================================
        private List<Sweet> SetupSweets(List<Category> categories)
        {
            return new List<Sweet>
            {
                new Sweet { Id = 1, Name = "Дубайский шоколад", Description = "Шоколад с фисташковой и кунжутной пастой", Category = categories.First(c => c.NormalizedName == "chocolates"), Price = 15.50m, Image = "/images/dubai-chocolate.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 2, Name = "Мармеладные мишки", Description = "Цветные мармеладные мишки", Category = categories.First(c => c.NormalizedName == "marmalade"), Price = 12.00m, Image = "/images/gummy-bears.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 3, Name = "Зефир ванильный", Description = "Нежный ванильный зефир", Category = categories.First(c => c.NormalizedName == "marshmallow"), Price = 18.75m, Image = "/images/vanilla-marshmallow.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 4, Name = "Печенье с шоколадом", Description = "Хрустящее печенье", Category = categories.First(c => c.NormalizedName == "cookies"), Price = 25.00m, Image = "/images/chocolate-cookies.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 5, Name = "Карамель", Description = "Сладкая карамель", Category = categories.First(c => c.NormalizedName == "candies"), Price = 8.00m, Image = "/images/caramel-candy.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 6, Name = "Торт", Description = "Вкусный торт", Category = categories.First(c => c.NormalizedName == "cakes"), Price = 35.00m, Image = "/images/cake.jpg", ContentType = "image/jpeg" }
            };
        }
    }
}
