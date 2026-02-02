using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab1.UI.Services.SweetService;
using Lab1.Domain.Models;
using Lab1.Domain.Entities;

namespace Lab1.UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ISweetService _sweetService;

        public IndexModel(ILogger<IndexModel> logger, ISweetService sweetService)
        {
            _logger = logger;
            _sweetService = sweetService;
        }

        public ResponseData<ListModel<Sweet>>? Sweets { get; private set; }

        public async Task OnGet()
        {
            Sweets = await _sweetService.GetSweetListAsync();
        }
    }
}
