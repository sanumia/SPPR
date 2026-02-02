using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NSubstitute;
using Xunit;
using Lab1.UI.Controllers;
using Lab1.UI.Services.SweetService;
using Lab1.UI.Services.CategoryService;
using Lab1.Domain.Models;
using Lab1.Domain.Entities;

namespace Lab1.Tests.Controllers
{
    public class SweetsControllerTests
    {
        private readonly SweetsController _controller;
        private readonly ISweetService _mockSweetService;
        private readonly ICategoryService _mockCategoryService;

        public SweetsControllerTests()
        {
            _mockSweetService = Substitute.For<ISweetService>();
            _mockCategoryService = Substitute.For<ICategoryService>();

            // Создаем контроллер с минимальными зависимостями
            _controller = CreateControllerWithContext();
        }

        private SweetsController CreateControllerWithContext()
        {
            var controller = new SweetsController(_mockSweetService, _mockCategoryService);

            // Минимальная настройка контекста
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            return controller;
        }

        [Fact]
        public async Task Index_ReturnsView_WithValidData()
        {
            // Arrange
            var sweets = new List<Sweet>
            {
                new Sweet { Id = 1, Name = "Test Sweet", Price = 10.0m, CategoryId = 1 }
            };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Шоколад", NormalizedName = "chocolates" }
            };

            var sweetResponse = ResponseData<ListModel<Sweet>>.Success(new ListModel<Sweet>
            {
                Items = sweets,
                TotalCount = 1,
                CurrentPage = 1,
                TotalPages = 1,
                PageSize = 3
            });

            var categoryResponse = ResponseData<List<Category>>.Success(categories);

            _mockSweetService.GetSweetListAsync(null, 1).Returns(sweetResponse);
            _mockCategoryService.GetCategoryListAsync().Returns(categoryResponse);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task Index_ReturnsView_WithCategoryFilter()
        {
            // Arrange
            var categoryName = "chocolates";
            var sweets = new List<Sweet>
            {
                new Sweet { Id = 1, Name = "Chocolate Sweet", Price = 15.0m, CategoryId = 1 }
            };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Шоколад", NormalizedName = "chocolates" }
            };

            var sweetResponse = ResponseData<ListModel<Sweet>>.Success(new ListModel<Sweet>
            {
                Items = sweets,
                TotalCount = 1,
                CurrentPage = 1,
                TotalPages = 1,
                PageSize = 3
            });

            var categoryResponse = ResponseData<List<Category>>.Success(categories);

            _mockSweetService.GetSweetListAsync(categoryName, 1).Returns(sweetResponse);
            _mockCategoryService.GetCategoryListAsync().Returns(categoryResponse);

            // Act
            var result = await _controller.Index(categoryName);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task Index_HandlesSweetServiceError()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Шоколад", NormalizedName = "chocolates" }
            };

            var sweetResponse = ResponseData<ListModel<Sweet>>.Error("Sweet service error");
            var categoryResponse = ResponseData<List<Category>>.Success(categories);

            _mockSweetService.GetSweetListAsync(null, 1).Returns(sweetResponse);
            _mockCategoryService.GetCategoryListAsync().Returns(categoryResponse);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task Index_HandlesCategoryServiceError()
        {
            // Arrange
            var sweets = new List<Sweet>
            {
                new Sweet { Id = 1, Name = "Test Sweet", Price = 10.0m, CategoryId = 1 }
            };

            var sweetResponse = ResponseData<ListModel<Sweet>>.Success(new ListModel<Sweet>
            {
                Items = sweets,
                TotalCount = 1,
                CurrentPage = 1,
                TotalPages = 1,
                PageSize = 3
            });

            var categoryResponse = ResponseData<List<Category>>.Error("Category service error");

            _mockSweetService.GetSweetListAsync(null, 1).Returns(sweetResponse);
            _mockCategoryService.GetCategoryListAsync().Returns(categoryResponse);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task Index_CorrectsPageNumber_WhenLessThanOne()
        {
            // Arrange
            var sweets = new List<Sweet>
            {
                new Sweet { Id = 1, Name = "Test Sweet", Price = 10.0m, CategoryId = 1 }
            };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Шоколад", NormalizedName = "chocolates" }
            };

            var sweetResponse = ResponseData<ListModel<Sweet>>.Success(new ListModel<Sweet>
            {
                Items = sweets,
                TotalCount = 1,
                CurrentPage = 1,
                TotalPages = 1,
                PageSize = 3
            });

            var categoryResponse = ResponseData<List<Category>>.Success(categories);

            _mockSweetService.GetSweetListAsync(null, 1).Returns(sweetResponse);
            _mockCategoryService.GetCategoryListAsync().Returns(categoryResponse);

            // Act - передаем pageNo = 0
            var result = await _controller.Index(null, 0);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        // Простой тест для AJAX - без сложной настройки
        [Fact]
        public async Task Index_ReturnsResult_ForNormalRequest()
        {
            // Arrange
            var sweets = new List<Sweet>
            {
                new Sweet { Id = 1, Name = "Test Sweet", Price = 10.0m, CategoryId = 1 }
            };

            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Шоколад", NormalizedName = "chocolates" }
            };

            var sweetResponse = ResponseData<ListModel<Sweet>>.Success(new ListModel<Sweet>
            {
                Items = sweets,
                TotalCount = 1,
                CurrentPage = 1,
                TotalPages = 1,
                PageSize = 3
            });

            var categoryResponse = ResponseData<List<Category>>.Success(categories);

            _mockSweetService.GetSweetListAsync(null, 1).Returns(sweetResponse);
            _mockCategoryService.GetCategoryListAsync().Returns(categoryResponse);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
    }
}