using System.Collections.Generic;
using Lab1.Domain.Entities;
using Lab1.Domain.Models;

namespace Lab1.Tests.TestHelpers
{
    public static class TestModelBuilder
    {
        public static ListModel<Sweet> CreateSweetListModel(List<Sweet> sweets, int totalCount, int currentPage, int pageSize)
        {
            return new ListModel<Sweet>
            {
                Items = sweets,
                TotalCount = totalCount,
                CurrentPage = currentPage,
                TotalPages = (totalCount + pageSize - 1) / pageSize,
                PageSize = pageSize
            };
        }

        public static Sweet CreateTestSweet(int id, string name, string categoryNormalizedName = "chocolates")
        {
            return new Sweet
            {
                Id = id,
                Name = name,
                Description = $"Description for {name}",
                Price = 10.0m + id,
                CategoryId = 1,
                Category = new Category
                {
                    Id = 1,
                    Name = "Шоколад",
                    NormalizedName = categoryNormalizedName
                }
            };
        }

        public static Category CreateTestCategory(int id, string name, string normalizedName)
        {
            return new Category
            {
                Id = id,
                Name = name,
                NormalizedName = normalizedName
            };
        }

        public static ResponseData<T> CreateSuccessResponse<T>(T data)
        {
            return new ResponseData<T>
            {
                Successfull = true,
                Data = data,
                ErrorMessage = null
            };
        }

        public static ResponseData<T> CreateErrorResponse<T>(string errorMessage)
        {
            return new ResponseData<T>
            {
                Successfull = false,
                Data = default(T),
                ErrorMessage = errorMessage
            };
        }
    }
}