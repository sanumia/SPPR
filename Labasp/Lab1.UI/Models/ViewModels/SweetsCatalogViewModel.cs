using System.Collections.Generic;
using Lab1.Domain.Entities;
using Lab1.Domain.Models;

namespace Lab1.UI.Models.ViewModels
{
    public class SweetsCatalogViewModel
    {
        public ListModel<Sweet> Sweets { get; set; } = new();
        public IReadOnlyCollection<Category> Categories { get; set; } = new List<Category>();
        public string? SelectedCategory { get; set; }
        public string? SelectedCategoryDisplayName { get; set; }
        public string? ErrorMessage { get; set; }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    }
}






