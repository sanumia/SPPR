using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.Domain.Models;

namespace Lab1.UI.Services.CategoryService
{
    public interface ICategoryService
    {
        public Task<ResponseData<List<Category>>> GetCategoryListAsync();
        public Task<ResponseData<Category>> GetCategoryByIdAsync(int id);
        public Task<ResponseData<Category>> CreateCategoryAsync(Category category);
        public Task<ResponseData<Category>> UpdateCategoryAsync(Category category);
        public Task<ResponseData<bool>> DeleteCategoryAsync(int id);
    }
}