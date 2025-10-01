using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab1.UI.Services.CategoryService;
using Lab1.UI.Services.SweetService;
using Lab1.Domain.Entities;
using Lab1.Domain.Models;

using Microsoft.AspNetCore.Mvc;

namespace Lab1.Controllers
{
    public class SweetController : Controller
    {
        private readonly ISweetService _sweetService;
        private readonly ICategoryService _categoryService;
        public SweetController(ISweetService sweetService, ICategoryService categoryService)
        {
            _sweetService = sweetService;
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index(string category = null, int pageNo = 1)
        {
            var sweetsResult = await _sweetService.GetSweetListAsync(category, pageNo);
            var categoriesResult = await _categoryService.GetCategoryListAsync();

            if (sweetsResult.Successfull && categoriesResult.Successfull)
            {
                ViewData["Categories"] = categoriesResult.Data;
                ViewData["CurrentCategory"] = string.IsNullOrEmpty(category)
                    ? "Все"
                    : categoriesResult.Data.FirstOrDefault(c => c.NormalizedName == category)?.Name ?? category;

                return View(sweetsResult.Data);
            }

            return View(new ListModel<Sweet> { Items = new List<Sweet>() });
        }
    }
}