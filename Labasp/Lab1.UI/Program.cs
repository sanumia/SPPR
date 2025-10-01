var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddScoped<Lab1.UI.Services.CategoryService.ICategoryService, Lab1.UI.Services.CategoryService.MemoryCategoryService>();
builder.Services.AddScoped<Lab1.UI.Services.SweetService.ISweetService, Lab1.UI.Services.SweetService.MemorySweetService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.UseSession();

app.Run();
