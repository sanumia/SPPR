using Lab1.API.Data;
using Lab1.API.EndPoints;
using Lab1.API.Interfaces;
using Lab1.API.Models;
using Lab1.UI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Загружаем appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Подключение к БД
var connectionString = builder.Configuration.GetConnectionString("Postgres");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string 'Postgres' not found in appsettings.json");
}
Console.WriteLine($"Connection string: {connectionString}");

// Регистрируем сервисы
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Получаем UriData из конфигурации
var uriData = builder.Configuration.GetSection("UriData").Get<UriData>();
if (uriData == null || string.IsNullOrEmpty(uriData.ApiUri))
{
    throw new Exception("UriData configuration is missing in appsettings.json");
}

// Регистрируем HttpClient для сервисов
builder.Services.AddHttpClient<ISweetService, ApiSweetService>(opt =>
    opt.BaseAddress = new Uri(uriData.ApiUri + "sweets/"));

builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(opt =>
    opt.BaseAddress = new Uri(uriData.ApiUri + "categories/"));

// Регистрируем MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

var app = builder.Build();

// Middleware pipeline
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Инициализация БД
await DbInitializer.SeedData(app);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapSweetEndpoints();

app.Run();