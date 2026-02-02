using Lab1.Domain.Models;

namespace Lab1.UI.Models.ViewModels
{
    public class CartViewModel
    {
        public Cart Cart { get; set; } = new();
        public string ReturnUrl { get; set; } = "/";
    }
}

