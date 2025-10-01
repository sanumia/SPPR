using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Lab1.UI.Services.CategoryService;
using Lab1.UI.Services.SweetService;



namespace Lab1.UI.Extensions
{
    public static class HostingExtensions
    {
        public static void RegisterCustomServices(this WebApplicationBuilder builder)
        {
            // Регистрация стандартных сервисов MVC
            builder.Services.AddControllersWithViews();
            
            // Регистрация ICategoryService как scoped сервиса
            builder.Services.AddScoped<ICategoryService, MemoryCategoryService>();
            builder.Services.AddScoped<ISweetService, MemorySweetService>();
            // Дополнительные сервисы (если нужны)
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();
        }
    }
}