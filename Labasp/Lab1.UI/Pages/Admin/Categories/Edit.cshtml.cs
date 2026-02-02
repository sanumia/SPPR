using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.UI.Services.CategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.UI.Pages.Admin.Categories
{
    [Authorize(Policy = "admin")]
    public class EditModel : PageModel
    {
        private readonly ICategoryService _service;
        public EditModel(ICategoryService service)
        {
            _service = service;
        }

        [BindProperty]
        public Category Item { get; set; } = new Category { Name = string.Empty, NormalizedName = string.Empty };

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _service.GetCategoryByIdAsync(id);
            if (!response.Successfull || response.Data == null) return NotFound();
            Item = response.Data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var result = await _service.UpdateCategoryAsync(Item);
            if (!result.Successfull)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Ошибка обновления");
                return Page();
            }
            return RedirectToPage("Index");
        }
    }
}


