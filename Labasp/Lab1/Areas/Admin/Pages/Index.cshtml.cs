using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.UI.Services.CategoryService;
using Lab1.UI.Services.SweetService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.Areas.Admin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly ISweetService _sweetService;

        public IndexModel(ICategoryService categoryService, ISweetService sweetService)
        {
            _categoryService = categoryService;
            _sweetService = sweetService;
        }

        [BindProperty(SupportsGet = true)]
        public string ActiveTab { get; set; } = "categories";

        public List<Category> Categories { get; set; } = new();
        public List<Sweet> Sweets { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var cats = await _categoryService.GetCategoryListAsync();
            if (cats.Success)
            {
                Categories = cats.Data;
            }

            var sweets = await _sweetService.GetSweetListAsync();
            if (sweets.Success && sweets.Data?.Items != null)
            {
                Sweets = sweets.Data.Items;
            }
        }

        public async Task<IActionResult> OnPostCreateCategoryAsync([FromForm] Category category)
        {
            await _categoryService.CreateCategoryAsync(category);
            ActiveTab = "categories";
            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateCategoryAsync([FromForm] Category category)
        {
            await _categoryService.UpdateCategoryAsync(category);
            ActiveTab = "categories";
            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteCategoryAsync(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            ActiveTab = "categories";
            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateSweetAsync([FromForm] Sweet sweet)
        {
            await _sweetService.CreateSweetAsync(sweet);
            ActiveTab = "sweets";
            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateSweetAsync([FromForm] Sweet sweet)
        {
            await _sweetService.UpdateSweetAsync(sweet);
            ActiveTab = "sweets";
            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteSweetAsync(int id)
        {
            await _sweetService.DeleteSweetAsync(id);
            ActiveTab = "sweets";
            await LoadDataAsync();
            return Page();
        }
    }
}


