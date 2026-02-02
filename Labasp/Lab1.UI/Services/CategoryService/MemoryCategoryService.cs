using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.Domain.Models;

namespace Lab1.UI.Services.CategoryService
{
    public class MemoryCategoryService : ICategoryService
    {
        private readonly List<Category> _categories = new()
        {
            new Category {Id=1, Name="Шоколад", NormalizedName="chocolates"},
            new Category {Id=2, Name="Конфеты", NormalizedName="candies"},
            new Category {Id=3, Name="Мармелад", NormalizedName="marmalade"},
            new Category {Id=4, Name="Зефир", NormalizedName="marshmallow"},
            new Category {Id=5, Name="Печенье", NormalizedName="cookies"},
            new Category {Id=6, Name="Орехи", NormalizedName="nuts"}
        };

        public Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var result = ResponseData<List<Category>>.Success(_categories.ToList());
            return Task.FromResult(result);
        }

        public Task<ResponseData<Category>> GetCategoryByIdAsync(int id)
        {
            var category = _categories.FirstOrDefault(c => c.Id == id);
            return category == null
                ? Task.FromResult(ResponseData<Category>.Error($"Category with id {id} not found"))
                : Task.FromResult(ResponseData<Category>.Success(category));
        }

        public Task<ResponseData<Category>> CreateCategoryAsync(Category category)
        {
            if (category == null)
            {
                return Task.FromResult(ResponseData<Category>.Error("Invalid category payload"));
            }

            category.Id = _categories.Count == 0 ? 1 : _categories.Max(c => c.Id) + 1;
            if (string.IsNullOrWhiteSpace(category.NormalizedName) && !string.IsNullOrWhiteSpace(category.Name))
            {
                category.NormalizedName = category.Name.Trim().ToLowerInvariant().Replace(' ', '-');
            }
            _categories.Add(category);
            return Task.FromResult(ResponseData<Category>.Success(category));
        }

        public Task<ResponseData<Category>> UpdateCategoryAsync(Category category)
        {
            var existing = _categories.FirstOrDefault(c => c.Id == category.Id);
            if (existing == null)
            {
                return Task.FromResult(ResponseData<Category>.Error($"Category with id {category.Id} not found"));
            }

            existing.Name = category.Name;
            existing.NormalizedName = string.IsNullOrWhiteSpace(category.NormalizedName)
                ? existing.NormalizedName
                : category.NormalizedName;

            return Task.FromResult(ResponseData<Category>.Success(existing));
        }

        public Task<ResponseData<bool>> DeleteCategoryAsync(int id)
        {
            var category = _categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return Task.FromResult(ResponseData<bool>.Error($"Category with id {id} not found"));
            }

            _categories.Remove(category);
            return Task.FromResult(ResponseData<bool>.Success(true));
        }
    }
}