using Lab1.BlazorWasm;
using Lab1.BlazorWasm.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Получаем базовый адрес API из конфигурации
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7002/";

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>()
.ConfigureHttpClient(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Регистрация DataService
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<SeedDataService>();
// Настройка авторизации OIDC
builder.Services.AddOidcAuthentication(options =>
{
    var keycloakConfig = builder.Configuration.GetSection("Keycloak");
    
    options.ProviderOptions.Authority = keycloakConfig["Authority"];
    options.ProviderOptions.ClientId = keycloakConfig["ClientId"];
    options.ProviderOptions.ResponseType = keycloakConfig["ResponseType"] ?? "code";
    
    // Настройка Redirect URIs
    var baseAddress = builder.HostEnvironment.BaseAddress.TrimEnd('/');
    options.ProviderOptions.RedirectUri = $"{baseAddress}/authentication/login-callback";
    options.ProviderOptions.PostLogoutRedirectUri = baseAddress;
    
    // Scopes
    options.ProviderOptions.DefaultScopes.Clear();
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
    
    // Дополнительные настройки для Keycloak
    options.UserOptions.NameClaim = "preferred_username";
    options.UserOptions.RoleClaim = "roles";
    
    // Настройки для работы с Keycloak
    options.ProviderOptions.ResponseMode = "query";
});

// Регистрация HttpClient для DataService (требует авторизации)
builder.Services.AddScoped(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return httpClientFactory.CreateClient("ApiClient");
});

await builder.Build().RunAsync();
