using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.UI.Services.CategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.UI.Pages.Admin.Categories
{
    [Authorize(Policy = "admin")]
    public class DeleteModel : PageModel
    {
        private readonly ICategoryService _service;
        public DeleteModel(ICategoryService service)
        {
            _service = service;
        }

        [BindProperty]
        public Category? Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _service.GetCategoryByIdAsync(id);
            if (!response.Successfull || response.Data == null) return NotFound();
            Item = response.Data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Item == null) return NotFound();
            var result = await _service.DeleteCategoryAsync(Item.Id);
            if (!result.Successfull)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Ошибка удаления");
                return Page();
            }
            return RedirectToPage("Index");
        }
    }
}


