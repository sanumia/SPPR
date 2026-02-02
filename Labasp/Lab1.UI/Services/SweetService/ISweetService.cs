using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Lab1.UI.Services.SweetService
{
    public interface ISweetService
    {
        /// <summary>
        /// Получение списка всех сладостей
        /// </summary>
        /// <returns>Список всех сладостей</returns>

            Task<ResponseData<ListModel<Sweet>>> GetSweetListAsync(string categoryNormalizedName = null, int pageNo = 1);

            Task<ResponseData<Sweet>> GetSweetByIdAsync(int id);
            Task<ResponseData<Sweet>> CreateSweetAsync(Sweet sweet, IFormFile? image = null);
            Task<ResponseData<Sweet>> UpdateSweetAsync(Sweet sweet, IFormFile? image = null);
            Task<ResponseData<bool>> DeleteSweetAsync(int id);

    }
}