using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Lab1.UI.Services.CategoryService;
using Lab1.Domain.Models;
using Lab1.Domain.Entities;

namespace Lab1.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly MemoryCategoryService _service;

        public CategoryServiceTests()
        {
            _service = new MemoryCategoryService();
        }

        [Fact]
        public async Task GetCategoryListAsync_ReturnsAllCategories()
        {
            // Act
            var result = await _service.GetCategoryListAsync();

            // Assert
            Assert.True(result.Successfull);
            Assert.NotNull(result.Data);
            Assert.Equal(6, result.Data.Count);
            Assert.Contains(result.Data, c => c.NormalizedName == "chocolates");
            Assert.Contains(result.Data, c => c.NormalizedName == "candies");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsCategory_WhenExists()
        {
            // Act
            var result = await _service.GetCategoryByIdAsync(1);

            // Assert
            Assert.True(result.Successfull);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("Шоколад", result.Data.Name);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsError_WhenNotFound()
        {
            // Act
            var result = await _service.GetCategoryByIdAsync(999);

            // Assert
            Assert.False(result.Successfull);
            Assert.Contains("not found", result.ErrorMessage);
        }

        [Fact]
        public async Task CreateCategoryAsync_CreatesCategory_WithAutoIncrementId()
        {
            // Arrange
            var newCategory = new Category { Name = "Test Category", Description = "Test Description" };

            // Act
            var result = await _service.CreateCategoryAsync(newCategory);

            // Assert
            Assert.True(result.Successfull);
            Assert.Equal(7, result.Data.Id); // Next ID after 6 existing categories
            Assert.Equal("Test Category", result.Data.Name);
            Assert.Equal("test-category", result.Data.NormalizedName);
        }

        [Fact]
        public async Task CreateCategoryAsync_GeneratesNormalizedName_WhenNotProvided()
        {
            // Arrange
            var newCategory = new Category { Name = "Test Category With Spaces" };

            // Act
            var result = await _service.CreateCategoryAsync(newCategory);

            // Assert
            Assert.True(result.Successfull);
            Assert.Equal("test-category-with-spaces", result.Data.NormalizedName);
        }

        [Fact]
        public async Task CreateCategoryAsync_ReturnsError_ForNullCategory()
        {
            // Act
            var result = await _service.CreateCategoryAsync(null);

            // Assert
            Assert.False(result.Successfull);
            Assert.Contains("Invalid category payload", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateCategoryAsync_UpdatesCategory_WhenExists()
        {
            // Arrange
            var existingCategory = (await _service.GetCategoryByIdAsync(1)).Data;
            existingCategory.Name = "Updated Category Name";

            // Act
            var result = await _service.UpdateCategoryAsync(existingCategory);

            // Assert
            Assert.True(result.Successfull);
            Assert.Equal("Updated Category Name", result.Data.Name);
            Assert.Equal(1, result.Data.Id);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ReturnsError_WhenCategoryNotFound()
        {
            // Arrange
            var nonExistingCategory = new Category { Id = 999, Name = "Non-existing" };

            // Act
            var result = await _service.UpdateCategoryAsync(nonExistingCategory);

            // Assert
            Assert.False(result.Successfull);
            Assert.Contains("not found", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteCategoryAsync_DeletesCategory_WhenExists()
        {
            // Act
            var result = await _service.DeleteCategoryAsync(1);

            // Assert
            Assert.True(result.Successfull);
            Assert.True(result.Data);

            // Verify category is deleted
            var getResult = await _service.GetCategoryByIdAsync(1);
            Assert.False(getResult.Successfull);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ReturnsError_WhenCategoryNotFound()
        {
            // Act
            var result = await _service.DeleteCategoryAsync(999);

            // Assert
            Assert.False(result.Successfull);
            Assert.Contains("not found", result.ErrorMessage);
        }
    }
}