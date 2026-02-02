using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.UI.Services.CategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.UI.Pages.Admin.Categories
{
    [Authorize(Policy = "admin")]
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _service;
        public CreateModel(ICategoryService service)
        {
            _service = service;
        }

        [BindProperty]
        public Category Item { get; set; } = new Category { Name = string.Empty, NormalizedName = string.Empty };

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var result = await _service.CreateCategoryAsync(Item);
            if (!result.Successfull)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Ошибка добавления");
                return Page();
            }
            return RedirectToPage("Index");
        }
    }
}


