using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Lab1.UI.Services.SweetService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab1.UI.Pages.Admin.Sweets
{
    [Authorize(Policy = "admin")]
    public class IndexModel : PageModel
    {
        private readonly ISweetService _sweetService;

        public IndexModel(ISweetService sweetService)
        {
            _sweetService = sweetService;
        }

        public IList<Sweet> Items { get; private set; } = new List<Sweet>();
        public int CurrentPage { get; private set; } = 1;
        public int TotalPages { get; private set; } = 1;

        public async Task OnGetAsync(int pageNo = 1)
        {
            var result = await _sweetService.GetSweetListAsync(pageNo: pageNo);
            if (result.Successfull)
            {
                Items = result.Data.Items;
                CurrentPage = result.Data.CurrentPage;
                TotalPages = result.Data.TotalPages;
            }
        }
    }
}


