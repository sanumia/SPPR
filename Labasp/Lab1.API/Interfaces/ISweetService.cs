using Lab1.API.Models;
using Lab1.Domain.Entities;

namespace Lab1.API.Interfaces
{
    public interface ISweetService
    {
        Task<ResponseData<ListModel<Sweet>>> GetSweetListAsync(string? categoryNormalizedName, int pageNo = 1);
        Task<ResponseData<Sweet>> GetSweetByIdAsync(int id);
        Task<ResponseData<Sweet>> CreateSweetAsync(Sweet sweet);
        Task<ResponseData<Sweet>> UpdateSweetAsync(int id, Sweet sweet);
        Task<ResponseData<bool>> DeleteSweetAsync(int id);
    }
}
