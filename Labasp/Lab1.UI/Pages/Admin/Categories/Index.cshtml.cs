using System.Collections.Generic;
using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.UI.Services.CategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.UI.Pages.Admin.Categories
{
    [Authorize(Policy = "admin")]
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _service;
        public List<Category> Items { get; private set; } = new();

        public IndexModel(ICategoryService service)
        {
            _service = service;
        }

        public async Task OnGet()
        {
            var response = await _service.GetCategoryListAsync();
            if (response.Successfull)
            {
                Items = response.Data;
            }
        }
    }
}


