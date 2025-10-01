using Lab1.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab1.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Выполняем миграции
            await context.Database.MigrateAsync();

            // Получаем ApiBaseUrl из appsettings.json
            var config = app.Configuration;
            var apiBaseUrl = config["ApiBaseUrl"] ?? "https://localhost:7002";

            // Если категорий нет – добавляем
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Шоколад", NormalizedName = "chocolates", Description = "Разные виды шоколада" },
                    new Category { Name = "Мармелад", NormalizedName = "marmalade", Description = "Фруктовый мармелад" },
                    new Category { Name = "Зефир", NormalizedName = "marshmallow", Description = "Мягкий зефир" },
                    new Category { Name = "Печенье", NormalizedName = "cookies", Description = "Свежее печенье" },
                    new Category { Name = "Конфеты", NormalizedName = "candies", Description = "Сладкие конфеты" },
                    new Category { Name = "Торты", NormalizedName = "cakes", Description = "Праздничные торты" }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Если сладостей нет – добавляем
            if (!context.Sweets.Any())
            {
                var categories = context.Categories.ToList();

                var sweets = new List<Sweet>
                {
                    new Sweet
                    {
                        Name = "Дубайский шоколад",
                        Description = "Шоколад с фисташковой и кунжутной пастой",
                        CategoryId = categories.First(c => c.NormalizedName == "chocolates").Id,
                        Price = 15.50m,
                        Image = $"{apiBaseUrl}/images/dubai-chocolate.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Name = "Мармеладные мишки",
                        Description = "Цветные мармеладные мишки",
                        CategoryId = categories.First(c => c.NormalizedName == "marmalade").Id,
                        Price = 12.00m,
                        Image = $"{apiBaseUrl}/images/gummy-bears.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Name = "Зефир ванильный",
                        Description = "Нежный ванильный зефир",
                        CategoryId = categories.First(c => c.NormalizedName == "marshmallow").Id,
                        Price = 18.75m,
                        Image = $"{apiBaseUrl}/images/vanilla-marshmallow.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Name = "Печенье с шоколадом",
                        Description = "Хрустящее печенье",
                        CategoryId = categories.First(c => c.NormalizedName == "cookies").Id,
                        Price = 25.00m,
                        Image = $"{apiBaseUrl}/images/chocolate-cookies.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Name = "Карамель",
                        Description = "Сладкая карамель",
                        CategoryId = categories.First(c => c.NormalizedName == "candies").Id,
                        Price = 8.00m,
                        Image = $"{apiBaseUrl}/images/caramel-candy.jpg",
                        ContentType = "image/jpeg"
                    },
                    new Sweet
                    {
                        Name = "Торт",
                        Description = "Вкусный торт",
                        CategoryId = categories.First(c => c.NormalizedName == "cakes").Id,
                        Price = 35.00m,
                        Image = $"{apiBaseUrl}/images/cake.jpg",
                        ContentType = "image/jpeg"
                    }
                };

                context.Sweets.AddRange(sweets);
                await context.SaveChangesAsync();
            }
        }
    }
}
