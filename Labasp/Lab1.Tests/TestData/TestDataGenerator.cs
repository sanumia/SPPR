using System.Collections.Generic;
using Lab1.Domain.Entities;
using Lab1.Domain.Models;

namespace Lab1.Tests.TestData
{
    public static class TestDataGenerator
    {
        public static List<Sweet> GetTestSweets()
        {
            return new List<Sweet>
            {
                new Sweet { Id = 1, Name = "Test Sweet 1", Price = 10.0m, CategoryId = 1 },
                new Sweet { Id = 2, Name = "Test Sweet 2", Price = 15.0m, CategoryId = 1 },
                new Sweet { Id = 3, Name = "Test Sweet 3", Price = 20.0m, CategoryId = 2 },
                new Sweet { Id = 4, Name = "Test Sweet 4", Price = 25.0m, CategoryId = 2 },
                new Sweet { Id = 5, Name = "Test Sweet 5", Price = 30.0m, CategoryId = 3 },
                new Sweet { Id = 6, Name = "Test Sweet 6", Price = 35.0m, CategoryId = 3 }
            };
        }

        public static List<Category> GetTestCategories()
        {
            return new List<Category>
            {
                new Category { Id = 1, Name = "Test Category 1", NormalizedName = "test-category-1" },
                new Category { Id = 2, Name = "Test Category 2", NormalizedName = "test-category-2" },
                new Category { Id = 3, Name = "Test Category 3", NormalizedName = "test-category-3" }
            };
        }

        public static ListModel<Sweet> GetTestSweetListModel()
        {
            return new ListModel<Sweet>
            {
                Items = GetTestSweets(),
                TotalCount = 6,
                CurrentPage = 1,
                TotalPages = 2,
                PageSize = 3
            };
        }

        public static ResponseData<ListModel<Sweet>> GetSuccessSweetResponse()
        {
            return ResponseData<ListModel<Sweet>>.Success(GetTestSweetListModel());
        }

        public static ResponseData<List<Category>> GetSuccessCategoryResponse()
        {
            return ResponseData<List<Category>>.Success(GetTestCategories());
        }
    }
}