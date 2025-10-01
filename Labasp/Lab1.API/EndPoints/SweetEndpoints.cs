using Microsoft.EntityFrameworkCore;
using Lab1.API.Data;
using Lab1.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using MediatR;
using Lab1.API.Models;

namespace Lab1.API.EndPoints;

public static class SweetEndpoints
{
    public static void MapSweetEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/sweets").WithTags(nameof(Sweet));

        // ✅ ОДИН универсальный endpoint для всех случаев
        group.MapGet("/{category:alpha?}",
            async (IMediator mediator, string? category, int pageNo = 1, int pageSize = 3) =>
            {
                var data = await mediator.Send(new GetListOfSweets(category, pageNo, pageSize));
                return TypedResults.Ok(data);
            })
            .WithName("GetSweets")
            .WithOpenApi()
            .Produces<ResponseData<ListModel<Sweet>>>(200);

        // Остальные CRUD endpoints
        group.MapGet("/by-id/{id}", async Task<Results<Ok<Sweet>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Sweets.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Sweet model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetSweetById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Sweet sweet, AppDbContext db) =>
        {
            var affected = await db.Sweets
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Name, sweet.Name)
                    .SetProperty(m => m.Description, sweet.Description)
                    .SetProperty(m => m.Price, sweet.Price)
                    .SetProperty(m => m.Image, sweet.Image)
                    .SetProperty(m => m.ContentType, sweet.ContentType)
                    .SetProperty(m => m.CategoryId, sweet.CategoryId));
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateSweet")
        .WithOpenApi();

        group.MapPost("/", async (Sweet sweet, AppDbContext db) =>
        {
            db.Sweets.Add(sweet);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/sweets/{sweet.Id}", sweet);
        })
        .WithName("CreateSweet")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Sweets
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteSweet")
        .WithOpenApi();
    }
}