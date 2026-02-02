using System.Threading.Tasks;
using Lab1.Domain.Models;
using Lab1.UI.Models.ViewModels;
using Lab1.UI.Services.SweetService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab1.UI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class CartController : Controller
    {
        private readonly ISweetService _sweetService;
        private readonly Cart _cart;

        public CartController(ISweetService sweetService, Cart cart)
        {
            _sweetService = sweetService;
            _cart = cart;
        }

        [HttpGet("")]
        public IActionResult Index([FromQuery] string returnUrl = "/")
        {
            var model = new CartViewModel
            {
                Cart = _cart,
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost("add/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, [FromForm] string returnUrl = "/")
        {
            var response = await _sweetService.GetSweetByIdAsync(id);
            if (response.Successfull && response.Data != null)
            {
                _cart.AddToCart(response.Data);
            }

            return Redirect(returnUrl);
        }

        [HttpPost("remove/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id, [FromForm] string returnUrl = "/")
        {
            _cart.RemoveItems(id);
            return Redirect(returnUrl);
        }

        [HttpPost("clear")]
        [ValidateAntiForgeryToken]
        public IActionResult Clear([FromForm] string returnUrl = "/")
        {
            _cart.ClearAll();
            return Redirect(returnUrl);
        }
    }
}

