using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Lab1.ViewComponents
{
    public class CartViewComponent:ViewComponent
    {
         public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartInfo = new
            {
                TotalPrice = 0.0m, 
                ItemCount = 0 
            };

            return View(cartInfo);
        }
    }
}