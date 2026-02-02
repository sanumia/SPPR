using System.Linq;
using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.Domain.Models;
using Lab1.UI.Extensions;
using Lab1.UI.Models.ViewModels;
using Lab1.UI.Services.CategoryService;
using Lab1.UI.Services.SweetService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab1.UI.Controllers
{
    [AllowAnonymous]
    public class SweetsController : Controller
    {
        private readonly ISweetService _sweetService;
        private readonly ICategoryService _categoryService;

        public SweetsController(
            ISweetService sweetService,
            ICategoryService categoryService)
        {
            _sweetService = sweetService;
            _categoryService = categoryService;
        }

        [HttpGet("Catalog")]
        [HttpGet("Catalog/{category?}")]
        public async Task<IActionResult> Index([FromRoute] string? category = null, [FromQuery] int pageNo = 1)
        {
            if (pageNo < 1) pageNo = 1;

            // Параллельные запросы к API
            var sweetsTask = _sweetService.GetSweetListAsync(category, pageNo);
            var categoriesTask = _categoryService.GetCategoryListAsync();
            await Task.WhenAll(sweetsTask, categoriesTask);

            var sweetsResponse = await sweetsTask;
            var categoriesResponse = await categoriesTask;

            // Формируем ViewModel
            var viewModel = new SweetsCatalogViewModel
            {
                Sweets = sweetsResponse.Successfull && sweetsResponse.Data != null
                    ? sweetsResponse.Data
                    : new ListModel<Sweet>(),

                Categories = categoriesResponse.Successfull && categoriesResponse.Data != null
                    ? categoriesResponse.Data
                    : new System.Collections.Generic.List<Category>(),

                SelectedCategory = category,
                ErrorMessage = !sweetsResponse.Successfull
                    ? sweetsResponse.ErrorMessage
                    : (!categoriesResponse.Successfull ? categoriesResponse.ErrorMessage : null)
            };

            // Определяем отображаемое имя категории
            if (!string.IsNullOrEmpty(category) && viewModel.Categories.Any())
            {
                var matched = viewModel.Categories.FirstOrDefault(c =>
                    c.NormalizedName.Equals(category, System.StringComparison.OrdinalIgnoreCase));
                viewModel.SelectedCategoryDisplayName = matched?.Name ?? "Категория";
            }
            else
            {
                viewModel.SelectedCategoryDisplayName = "Все категории";
            }

            // Поддержка AJAX-запросов (например, пагинация)
            if (Request.IsAjaxRequest())
            {
                return PartialView("_SweetsListPartial", viewModel);
            }

            return View(viewModel);
        }
    }
}
