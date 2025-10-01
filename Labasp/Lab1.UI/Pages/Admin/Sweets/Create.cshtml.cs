using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.UI.Services.SweetService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.UI.Pages.Admin.Sweets
{
    public class CreateModel : PageModel
    {
        private readonly ISweetService _sweetService;

        public CreateModel(ISweetService sweetService)
        {
            _sweetService = sweetService;
        }

        [BindProperty]
        public Sweet Item { get; set; } = new Sweet();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _sweetService.CreateSweetAsync(Item);
            if (!result.Successfull)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Ошибка сохранения");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}


