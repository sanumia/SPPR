using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.Domain.Models;
using Lab1.UI.Services.CategoryService;
using Microsoft.Extensions.Configuration;

namespace Lab1.UI.Services.SweetService
{
    public class MemorySweetService : ISweetService
    {
        private List<Sweet> _sweets;
        private List<Category> _categories;
        private readonly IConfiguration _config;

        public MemorySweetService(IConfiguration config, ICategoryService categoryService)
        {
            _config = config;
            _categories = categoryService.GetCategoryListAsync()
                .Result
                .Data;
            SetupData();
        }

        public Task<ResponseData<ListModel<Sweet>>> GetSweetListAsync(string categoryNormalizedName = null, int pageNo = 1)
        {
            int itemsPerPage = _config.GetValue<int>("ItemsPerPage", 3);

            // Фильтрация по категории
            var filteredSweets = string.IsNullOrEmpty(categoryNormalizedName)
                ? _sweets
                : _sweets.Where(s => s.Category != null &&
                                     s.Category.NormalizedName.Equals(categoryNormalizedName, StringComparison.OrdinalIgnoreCase))
                         .ToList();

            int totalCount = filteredSweets.Count;
            int totalPages = (int)Math.Ceiling(totalCount / (double)itemsPerPage);

            var pagedSweets = filteredSweets
                .Skip((pageNo - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            var listModel = new ListModel<Sweet>
            {
                Items = pagedSweets,
                CurrentPage = pageNo,
                TotalPages = totalPages,
                CurrentCategory = categoryNormalizedName
            };

            return Task.FromResult(ResponseData<ListModel<Sweet>>.Success(listModel));
        }

        public Task<ResponseData<Sweet>> GetSweetByIdAsync(int id)
        {
            var sweet = _sweets.FirstOrDefault(s => s.Id == id);
            if (sweet == null)
            {
                return Task.FromResult(ResponseData<Sweet>.Error($"Sweet with id {id} not found"));
            }
            return Task.FromResult(ResponseData<Sweet>.Success(sweet));
        }

        public Task<ResponseData<Sweet>> CreateSweetAsync(Sweet sweet)
        {
            if (sweet == null)
            {
                return Task.FromResult(ResponseData<Sweet>.Error("Invalid sweet payload"));
            }

            sweet.Id = _sweets.Count == 0 ? 1 : _sweets.Max(s => s.Id) + 1;
            sweet.Category = _categories.FirstOrDefault(c => c.Id == sweet.CategoryId)
                              ?? _categories.FirstOrDefault(c => c.Id == _categories.First().Id)!;
            _sweets.Add(sweet);
            return Task.FromResult(ResponseData<Sweet>.Success(sweet));
        }

        public Task<ResponseData<Sweet>> UpdateSweetAsync(Sweet sweet)
        {
            var existing = _sweets.FirstOrDefault(s => s.Id == sweet.Id);
            if (existing == null)
            {
                return Task.FromResult(ResponseData<Sweet>.Error($"Sweet with id {sweet.Id} not found"));
            }

            existing.Name = sweet.Name;
            existing.Description = sweet.Description;
            existing.Price = sweet.Price;
            existing.Image = sweet.Image;
            existing.ContentType = sweet.ContentType;
            existing.CategoryId = sweet.CategoryId;
            existing.Category = _categories.FirstOrDefault(c => c.Id == sweet.CategoryId) ?? existing.Category;

            return Task.FromResult(ResponseData<Sweet>.Success(existing));
        }

        public Task<ResponseData<bool>> DeleteSweetAsync(int id)
        {
            var sweet = _sweets.FirstOrDefault(s => s.Id == id);
            if (sweet == null)
            {
                return Task.FromResult(ResponseData<bool>.Error($"Sweet with id {id} not found"));
            }

            _sweets.Remove(sweet);
            return Task.FromResult(ResponseData<bool>.Success(true));
        }

        private void SetupData()
        {
            _sweets = new List<Sweet>
            {
                new Sweet { Id = 1, Name = "Дубайский шоколад", Description = "Шоколад с фисташковой и кунжутной пастой", Category = _categories.Find(c => c.NormalizedName == "chocolates")!, Price = 15.50m, Image = "/images/dubai-chocolate.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 2, Name = "Мармеладные мишки", Description = "Цветные мармеладные мишки", Category = _categories.Find(c => c.NormalizedName == "marmalade")!, Price = 12.00m, Image = "/images/gummy-bears.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 3, Name = "Зефир ванильный", Description = "Нежный ванильный зефир", Category = _categories.Find(c => c.NormalizedName == "marshmallow")!, Price = 18.75m, Image = "/images/vanilla-marshmallow.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 4, Name = "Печенье с шоколадом", Description = "Хрустящее печенье", Category = _categories.Find(c => c.NormalizedName == "cookies")!, Price = 25.00m, Image = "/images/chocolate-cookies.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 5, Name = "Карамель", Description = "Сладкая карамель", Category = _categories.Find(c => c.NormalizedName == "candies")!, Price = 8.00m, Image = "/images/caramel-candy.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 6, Name = "Торт", Description = "Вкусный торт", Category = _categories.Find(c => c.NormalizedName == "cakes")!, Price = 35.00m, Image = "/images/cake.jpg", ContentType = "image/jpeg" }
            };
        }
    }
}
