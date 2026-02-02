using System.Threading.Tasks;
using Lab1.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab1.UI.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly Cart _cart;

        public CartViewComponent(Cart cart)
        {
            _cart = cart;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View(_cart));
        }
    }
}

