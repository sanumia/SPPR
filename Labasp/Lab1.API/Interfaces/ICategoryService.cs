using Lab1.API.Models;
using Lab1.Domain.Entities;

namespace Lab1.API.Interfaces
{
    public interface ICategoryService
    {
        Task<ResponseData<ListModel<Category>>> GetCategoryListAsync(int pageNo = 1);
        Task<ResponseData<Category>> GetCategoryByIdAsync(int id);
        Task<ResponseData<Category>> CreateCategoryAsync(Category category);
        Task<ResponseData<Category>> UpdateCategoryAsync(int id, Category category);
        Task<ResponseData<bool>> DeleteCategoryAsync(int id);
    }
}
