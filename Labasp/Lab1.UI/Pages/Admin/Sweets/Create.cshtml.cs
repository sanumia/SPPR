using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.UI.Services.SweetService;
using Lab1.UI.Services.CategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Lab1.UI.Pages.Admin.Sweets
{
    [Authorize(Policy = "admin")]
    public class CreateModel : PageModel
    {
        private readonly ISweetService _sweetService;
        private readonly ICategoryService _categoryService;

        public CreateModel(ISweetService sweetService, ICategoryService categoryService)
        {
            _sweetService = sweetService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public Sweet Item { get; set; } = new Sweet();

        [BindProperty]
        public IFormFile? Image { get; set; }

        public List<SelectListItem> Categories { get; private set; } = new List<SelectListItem>();

        public async Task OnGet()
        {
            await LoadCategoriesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            var result = await _sweetService.CreateSweetAsync(Item, Image);
            if (!result.Successfull)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Ошибка сохранения");
                await LoadCategoriesAsync();
                return Page();
            }

            return RedirectToPage("Index");
        }

        private async Task LoadCategoriesAsync()
        {
            var resp = await _categoryService.GetCategoryListAsync();
            Categories.Clear();
            if (resp.Successfull && resp.Data != null)
            {
                foreach (var c in resp.Data)
                {
                    Categories.Add(new SelectListItem(c.Name, c.Id.ToString()));
                }
            }
        }
    }
}


