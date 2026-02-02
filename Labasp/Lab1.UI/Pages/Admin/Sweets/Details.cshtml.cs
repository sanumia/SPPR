using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.UI.Services.SweetService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.UI.Pages.Admin.Sweets
{
    [Authorize(Policy = "admin")]
    public class DetailsModel : PageModel
    {
        private readonly ISweetService _sweetService;

        public DetailsModel(ISweetService sweetService)
        {
            _sweetService = sweetService;
        }

        public Sweet Item { get; private set; } = new Sweet();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var result = await _sweetService.GetSweetByIdAsync(id);
            if (!(result.Successfull && result.Data != null))
            {
                return NotFound();
            }
            Item = result.Data;
            return Page();
        }
    }
}


