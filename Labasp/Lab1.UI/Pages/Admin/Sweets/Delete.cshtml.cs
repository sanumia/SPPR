using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.UI.Services.SweetService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.UI.Pages.Admin.Sweets
{
    [Authorize(Policy = "admin")]
    public class DeleteModel : PageModel
    {
        private readonly ISweetService _sweetService;

        public DeleteModel(ISweetService sweetService)
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
            var result = await _sweetService.DeleteSweetAsync(Item.Id);
            if (!result.Successfull)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Ошибка удаления");
                return Page();
            }
            return RedirectToPage("Index");
        }
    }
}


