using Lab1.API.Data;
using Lab1.API.EndPoints;
using Lab1.API.HelperClasses;
using Lab1.API.Models;
using Lab1.API.Services.FileService;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;

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
builder.Services.AddControllers(); // Для Web API
builder.Services.AddHttpContextAccessor();

// ✅ Настройка CORS для Blazor WebAssembly
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7275",
                "http://localhost:5043",
                "https://localhost:5043",
                "http://localhost:7275"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ✅ Регистрируем KeycloakData для Admin API (из своего appsettings.json)
var keycloakData = builder.Configuration.GetSection("Keycloak").Get<KeycloakData>();
if (keycloakData == null || string.IsNullOrEmpty(keycloakData.Host))
{
    throw new Exception("Keycloak configuration is missing in appsettings.json");
}
builder.Services.Configure<KeycloakData>(builder.Configuration.GetSection("Keycloak"));

// ✅ Регистрируем FileService для сохранения аватаров
builder.Services.AddScoped<IFileService, LocalFileService>();

// ✅ Регистрируем HttpClient для вызовов Keycloak Admin API
builder.Services.AddHttpClient();

// ✅ Регистрация Redis и HybridCache
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (string.IsNullOrWhiteSpace(redisConnection))
{
    throw new Exception("Redis connection string 'Redis' is missing in appsettings.json");
}

builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.InstanceName = "labs_";
    opt.Configuration = redisConnection;
});

builder.Services.AddHybridCache();

// Регистрируем MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

// ✅ Получаем AuthServerData для JWT аутентификации
var authServer = builder.Configuration.GetSection("AuthServer").Get<AuthServerData>();
if (authServer == null || string.IsNullOrEmpty(authServer.Host))
{
    throw new Exception("AuthServer configuration is missing in appsettings.json");
}

// ✅ Добавить сервис аутентификации JWT для защиты API
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
{
    // Конфигурация OpenID Connect из Keycloak
    o.MetadataAddress = $"{authServer.Host}/realms/{authServer.Realm}/.well-known/openid-configuration";
    o.Authority = $"{authServer.Host}/realms/{authServer.Realm}";

    // Аудитория должна совпадать с ClientId, который использует UI (`GorodetskayaUiClient`)
    var apiClientId = builder.Configuration["Keycloak:ClientId"] ?? "GorodetskayaUiClient";
    o.Audience = apiClientId;
    o.RequireHttpsMetadata = false;

    // Дополнительные настройки валидации токена
    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = apiClientId,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        NameClaimType = "preferred_username",
        RoleClaimType = "roles"
    };
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p => p.RequireRole("POWER-USER"));
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Для обслуживания изображений (аватаров)

// ✅ CORS должен быть до UseRouting и UseAuthentication
app.UseCors("AllowBlazorWasm");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Инициализация БД
await DbInitializer.SeedData(app);

// Маппинг для API контроллеров
app.MapControllers();

app.MapSweetEndpoints();

app.Run();