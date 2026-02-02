// SeedDataService.cs
using Lab1.Domain.Entities;

namespace Lab1.BlazorWasm.Services
{
    public class SeedDataService
    {
        public List<Category> GetDefaultCategories()
        {
            return new List<Category>
            {
                new Category { Id = 1, Name = "Шоколад", NormalizedName = "chocolates" },
                new Category { Id = 2, Name = "Мармелад", NormalizedName = "marmalade" },
                new Category { Id = 3, Name = "Зефир", NormalizedName = "marshmallow" },
                new Category { Id = 4, Name = "Печенье", NormalizedName = "cookies" },
                new Category { Id = 5, Name = "Конфеты", NormalizedName = "candies" },
                new Category { Id = 6, Name = "Торты", NormalizedName = "cakes" }
            };
        }

        public List<Sweet> GetDefaultSweets(List<Category> categories)
        {
            return new List<Sweet>
            {
                new Sweet { Id = 1, Name = "Дубайский шоколад", Description = "Шоколад с фисташковой и кунжутной пастой", Category = categories.First(c => c.NormalizedName == "chocolates"), Price = 15.50m, Image = "/images/dubai-chocolate.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 2, Name = "Мармеладные мишки", Description = "Цветные мармеладные мишки", Category = categories.First(c => c.NormalizedName == "marmalade"), Price = 12.00m, Image = "/images/gummy-bears.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 3, Name = "Зефир ванильный", Description = "Нежный ванильный зефир", Category = categories.First(c => c.NormalizedName == "marshmallow"), Price = 18.75m, Image = "/images/vanilla-marshmallow.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 4, Name = "Печенье с шоколадом", Description = "Хрустящее печенье", Category = categories.First(c => c.NormalizedName == "cookies"), Price = 25.00m, Image = "/images/chocolate-cookies.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 5, Name = "Карамель", Description = "Сладкая карамель", Category = categories.First(c => c.NormalizedName == "candies"), Price = 8.00m, Image = "/images/caramel-candy.jpg", ContentType = "image/jpeg" },
                new Sweet { Id = 6, Name = "Торт", Description = "Вкусный торт", Category = categories.First(c => c.NormalizedName == "cakes"), Price = 35.00m, Image = "/images/cake.jpg", ContentType = "image/jpeg" }
            };
        }
    }
}