using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.UI.Services.SweetService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.UI.Pages.Admin.Sweets
{
    public class EditModel : PageModel
    {
        private readonly ISweetService _sweetService;

        public EditModel(ISweetService sweetService)
        {
            _sweetService = sweetService;
        }

        [BindProperty]
        public Sweet Item { get; set; } = new Sweet();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var result = await _sweetService.GetSweetByIdAsync(id);
            if (!result.Successfull || result.Data == null)
            {
                return NotFound();
            }
            Item = result.Data;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _sweetService.UpdateSweetAsync(Item);
            if (!result.Successfull)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Ошибка обновления");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}


