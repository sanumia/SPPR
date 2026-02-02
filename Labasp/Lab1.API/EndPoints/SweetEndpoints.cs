using System;
using Lab1.API.Data;
using Microsoft.EntityFrameworkCore;
using Lab1.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using MediatR;
using Lab1.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;

namespace Lab1.API.EndPoints;

public static class SweetEndpoints
{
    public static void MapSweetEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/sweets")
            .WithTags(nameof(Sweet))
            .DisableAntiforgery();

        // ✅ GET endpoints - разрешаем анонимный доступ (чтение)
        group.MapGet("/{category:alpha?}",
            async (IMediator mediator,
                    HybridCache cache,
                    string? category,
                    int pageNo = 1,
                    int pageSize = 3) =>
            {
                var cacheKey = $"sweets_{category ?? "all"}_{pageNo}_{pageSize}";

                var data = await cache.GetOrCreateAsync(
                    cacheKey,
                    async token => await mediator.Send(new GetListOfSweets(category, pageNo, pageSize), token),
                    new HybridCacheEntryOptions
                    {
                        Expiration = TimeSpan.FromMinutes(1),
                        LocalCacheExpiration = TimeSpan.FromSeconds(30)
                    });

                return TypedResults.Ok(data);
            })
            .WithName("GetSweets")
            .WithOpenApi()
            .Produces<ResponseData<ListModel<Sweet>>>(200)
            .RequireAuthorization(); // Требуем авторизацию для просмотра

        group.MapGet("/by-id/{id}", async Task<Results<Ok<Sweet>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Sweets.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Sweet model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetSweetById")
        .WithOpenApi()
        .AllowAnonymous(); // Неавторизованный доступ на чтение

        // ⚠️ PUT endpoint - требует политику "admin"
        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (int id,
            [FromForm] string sweet,
            [FromForm] IFormFile? file,
            AppDbContext db,
            IMediator mediator) =>
        {
            var existing = await db.Sweets.FirstOrDefaultAsync(s => s.Id == id);
            if (existing == null) return TypedResults.NotFound();
            var updated = System.Text.Json.JsonSerializer.Deserialize<Sweet>(sweet);
            if (updated == null || updated.Id != id) return TypedResults.BadRequest();

            if (file != null && !string.IsNullOrWhiteSpace(existing.Image))
            {
                TryDeleteFile(existing.Image);
            }

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.Price = updated.Price;
            existing.CategoryId = updated.CategoryId;

            if (file != null)
            {
                existing.Image = await mediator.Send(new Lab1.API.UseCases.SaveImage(file));
                existing.ContentType = file.ContentType;
            }

            await db.SaveChangesAsync();
            return TypedResults.Ok();

            static void TryDeleteFile(string imageUrl)
            {
                try
                {
                    var fileName = System.IO.Path.GetFileName(new Uri(imageUrl, UriKind.RelativeOrAbsolute).IsAbsoluteUri
                        ? new Uri(imageUrl).AbsolutePath
                        : imageUrl);
                    var webRoot = AppContext.BaseDirectory;
                    var wwwroot = System.IO.Path.Combine(webRoot, "wwwroot");
                    var images = System.IO.Path.Combine(wwwroot, "images");
                    var path = System.IO.Path.Combine(images, fileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
                catch { }
            }
        })
        .WithName("UpdateSweet")
        .WithOpenApi()
        .RequireAuthorization("admin"); // Требуем политику admin

        // ⚠️ POST endpoint - требует политику "admin"
        group.MapPost("/", async Task<Results<Created<Sweet>, BadRequest>> ([FromForm] string sweet,
            [FromForm] IFormFile? file,
            AppDbContext db,
            IMediator mediator) =>
        {
            var newSweet = System.Text.Json.JsonSerializer.Deserialize<Sweet>(sweet);
            if (newSweet == null) return TypedResults.BadRequest();
            if (file != null)
            {
                newSweet.Image = await mediator.Send(new Lab1.API.UseCases.SaveImage(file));
                newSweet.ContentType = file.ContentType;
            }
            db.Sweets.Add(newSweet);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/sweets/{newSweet.Id}", newSweet);
        })
        .WithName("CreateSweet")
        .WithOpenApi()
        .RequireAuthorization("admin"); // Требуем политику admin

        // ⚠️ DELETE endpoint - требует политику "admin"
        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var sweetModel = await db.Sweets.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (sweetModel == null) return TypedResults.NotFound();
            if (!string.IsNullOrWhiteSpace(sweetModel.Image))
            {
                TryDeleteFile(sweetModel.Image);
            }
            var affected = await db.Sweets.Where(model => model.Id == id).ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();

            static void TryDeleteFile(string imageUrl)
            {
                try
                {
                    var fileName = System.IO.Path.GetFileName(new Uri(imageUrl, UriKind.RelativeOrAbsolute).IsAbsoluteUri
                        ? new Uri(imageUrl).AbsolutePath
                        : imageUrl);
                    var webRoot = AppContext.BaseDirectory;
                    var wwwroot = System.IO.Path.Combine(webRoot, "wwwroot");
                    var images = System.IO.Path.Combine(wwwroot, "images");
                    var path = System.IO.Path.Combine(images, fileName);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
                catch { }
            }
        })
        .WithName("DeleteSweet")
        .WithOpenApi()
        .RequireAuthorization("admin"); // Требуем политику admin
    }
}