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
        public Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var categories = new List<Category>
                {
                new Category {Id=1, Name="Шоколад",
                NormalizedName="chocolates"},
                new Category {Id=2, Name="Конфеты",
                NormalizedName="candies"},
                new Category {Id=3, Name="Мармелад", NormalizedName="marmalade"},
                new Category {Id=4, Name="Зефир",
                NormalizedName="marshmallow"},
                new Category {Id=5, Name="Печенье",
                NormalizedName="cookies"},
                new Category {Id=6, Name="Орехи",
                NormalizedName="nuts"}
                };
            var result = ResponseData<List<Category>>.Success(categories);
            return Task.FromResult(result);
        }
    }
}