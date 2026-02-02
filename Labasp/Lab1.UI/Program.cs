using Lab1.Domain.Models;
using Lab1.UI.HelperClasses;
using Lab1.UI.Services.Authentication;
using Lab1.UI.Services.CartService;
using Lab1.UI.Services.SweetService;
using Lab1.UI.Services.CategoryService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog;
using System.Security.Claims;
using Lab1.UI.Middleware;
using Lab1.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Serilog ------------------
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

builder.Host.UseSerilog((context, services, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day));

// ------------------ Services -----------------
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// ------------------ Keycloak Config -----------------
var keycloakData = builder.Configuration.GetSection("Keycloak").Get<KeycloakData>()
    ?? throw new Exception("Keycloak configuration is missing in appsettings.json");

builder.Services.Configure<KeycloakData>(builder.Configuration.GetSection("Keycloak"));

// ------------------ Authentication -----------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies", options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
})
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = $"{keycloakData.Host}/realms/{keycloakData.Realm}";
    options.ClientId = keycloakData.ClientId;
    options.ClientSecret = keycloakData.ClientSecret;
    options.ResponseType = OpenIdConnectResponseType.Code;

    options.CallbackPath = "/signin-oidc";
    options.SignedOutCallbackPath = "/signout-callback-oidc";

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("roles");

    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new()
    {
        NameClaimType = "preferred_username",
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
    {
        OnRedirectToIdentityProviderForSignOut = context =>
        {
            context.ProtocolMessage.PostLogoutRedirectUri = $"{context.Request.Scheme}://{context.Request.Host}/";
            return Task.CompletedTask;
        }
    };
});

// ------------------ Authorization -----------------
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p => p.RequireRole("POWER-USER"));
});

// ------------------ TokenAccessor & Handler -----------------
builder.Services.AddScoped<ITokenAccessor, KeycloakTokenAccessor>();
builder.Services.AddTransient<AuthorizationDelegatingHandler>();

// ------------------ HttpClients -----------------

var apiBaseUrl = builder.Configuration.GetValue<string>("Api:BaseUrl")
    ?? throw new Exception("Api:BaseUrl missing in configuration");

// Authenticated client — CUD операции
builder.Services.AddHttpClient<ISweetService, ApiSweetService>(c =>
{
    c.BaseAddress = new Uri(apiBaseUrl + "sweets/");
})
.AddHttpMessageHandler<AuthorizationDelegatingHandler>();

// Public client — GET операции (каталог)
builder.Services.AddHttpClient("PublicApi", c =>
{
    c.BaseAddress = new Uri(apiBaseUrl + "sweets/");
});

// Category service — обычно публичный
builder.Services.AddHttpClient<ICategoryService, ApiCategoryService>(c =>
{
    c.BaseAddress = new Uri(apiBaseUrl + "categories/");
});

// Дополнительный клиент для любых запросов к API
builder.Services.AddHttpClient("ApiClient", c =>
{
    c.BaseAddress = new Uri(apiBaseUrl);
});

// ------------------ Cart -----------------
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.RegisterCustomServices();

// ------------------ Build app -----------------
var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ------------------ Middleware -----------------
app.UseErrorRequestLogging();

// ------------------ Routes -----------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

// ------------------ Run -----------------
app.Run();
