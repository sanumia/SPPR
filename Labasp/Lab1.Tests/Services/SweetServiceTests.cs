using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;
using Lab1.UI.Services.SweetService;
using Lab1.UI.Services.CategoryService;
using Lab1.Domain.Models;
using Lab1.Domain.Entities;

namespace Lab1.Tests.Services
{
    public class SweetServiceTests
    {
        private readonly MemorySweetService _service;
        private readonly ICategoryService _mockCategoryService;

        public SweetServiceTests()
        {
            _mockCategoryService = Substitute.For<ICategoryService>();

            // Создаем реальную конфигурацию вместо мока
            var inMemorySettings = new Dictionary<string, string> {
                {"ItemsPerPage", "3"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            SetupCategoryServiceMock();
            _service = new MemorySweetService(configuration, _mockCategoryService);
        }

        private void SetupCategoryServiceMock()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Шоколад", NormalizedName = "chocolates" },
                new Category { Id = 2, Name = "Конфеты", NormalizedName = "candies" },
                new Category { Id = 3, Name = "Мармелад", NormalizedName = "marmalade" },
                new Category { Id = 4, Name = "Зефир", NormalizedName = "marshmallow" },
                new Category { Id = 5, Name = "Печенье", NormalizedName = "cookies" },
                new Category { Id = 6, Name = "Орехи", NormalizedName = "nuts" }
            };

            var categoriesResponse = ResponseData<List<Category>>.Success(categories);
            _mockCategoryService.GetCategoryListAsync().Returns(categoriesResponse);
        }

        [Fact]
        public async Task GetSweetListAsync_ReturnsPagedData()
        {
            // Act
            var result = await _service.GetSweetListAsync();

            // Assert
            Assert.True(result.Successfull);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Items);
            Assert.Equal(1, result.Data.CurrentPage);
            Assert.Equal(3, result.Data.PageSize);
        }

        [Fact]
        public async Task GetSweetListAsync_ReturnsSecondPage()
        {
            // Act
            var result = await _service.GetSweetListAsync(null, 2);

            // Assert
            Assert.True(result.Successfull);
            Assert.Equal(2, result.Data.CurrentPage);
        }

        [Fact]
        public async Task GetSweetListAsync_FiltersByCategory()
        {
            // Act
            var result = await _service.GetSweetListAsync("chocolates");

            // Assert
            Assert.True(result.Successfull);
            Assert.NotNull(result.Data.Items);
        }

        [Fact]
        public async Task GetSweetByIdAsync_ReturnsSweet_WhenExists()
        {
            // Act
            var result = await _service.GetSweetByIdAsync(1);

            // Assert
            Assert.True(result.Successfull);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
        }

        [Fact]
        public async Task GetSweetByIdAsync_ReturnsError_WhenNotExists()
        {
            // Act
            var result = await _service.GetSweetByIdAsync(999);

            // Assert
            Assert.False(result.Successfull);
            Assert.False(string.IsNullOrEmpty(result.ErrorMessage));
        }

        [Fact]
        public async Task CreateSweetAsync_CreatesNewSweet()
        {
            // Arrange
            var newSweet = new Sweet
            {
                Name = "New Test Sweet",
                Description = "Test Description",
                Price = 10.0m,
                CategoryId = 1
            };

            // Act
            var result = await _service.CreateSweetAsync(newSweet);

            // Assert
            Assert.True(result.Successfull);
            Assert.NotNull(result.Data);
            Assert.Equal("New Test Sweet", result.Data.Name);
            Assert.True(result.Data.Id > 0);
        }

        [Fact]
        public async Task CreateSweetAsync_ReturnsError_ForNullSweet()
        {
            // Act
            var result = await _service.CreateSweetAsync(null);

            // Assert
            Assert.False(result.Successfull);
            Assert.False(string.IsNullOrEmpty(result.ErrorMessage));
        }

        [Fact]
        public async Task UpdateSweetAsync_UpdatesExistingSweet()
        {
            // Arrange - создаем новую сладость для обновления
            var newSweet = new Sweet
            {
                Name = "Sweet to Update",
                Description = "Test",
                Price = 10.0m,
                CategoryId = 1
            };
            var created = await _service.CreateSweetAsync(newSweet);

            var sweetToUpdate = created.Data;
            sweetToUpdate.Name = "Updated Name";
            sweetToUpdate.Price = 99.99m;

            // Act
            var result = await _service.UpdateSweetAsync(sweetToUpdate);

            // Assert
            Assert.True(result.Successfull);
            Assert.Equal("Updated Name", result.Data.Name);
            Assert.Equal(99.99m, result.Data.Price);
        }

        [Fact]
        public async Task DeleteSweetAsync_DeletesExistingSweet()
        {
            // Arrange - создаем сладость для удаления
            var newSweet = new Sweet
            {
                Name = "Sweet to Delete",
                Description = "Test",
                Price = 10.0m,
                CategoryId = 1
            };
            var created = await _service.CreateSweetAsync(newSweet);
            var sweetId = created.Data.Id;

            // Act
            var result = await _service.DeleteSweetAsync(sweetId);

            // Assert
            Assert.True(result.Successfull);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteSweetAsync_ReturnsError_ForNonExistingSweet()
        {
            // Act
            var result = await _service.DeleteSweetAsync(999);

            // Assert
            Assert.False(result.Successfull);
            Assert.False(string.IsNullOrEmpty(result.ErrorMessage));
        }
    }
}