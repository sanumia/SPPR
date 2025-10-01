// Use-Cases/GetListOfSweets.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Lab1.API.Data;
using Lab1.Domain.Entities;
using Lab1.API.Models;

public sealed record GetListOfSweets(
    string? categoryNormalizedName,
    int pageNo = 1,
    int pageSize = 3) : IRequest<ResponseData<ListModel<Sweet>>>;

public class GetListOfSweetsHandler : IRequestHandler<GetListOfSweets, ResponseData<ListModel<Sweet>>>
{
    private readonly AppDbContext _db;
    private readonly int _maxPageSize = 20;

    public GetListOfSweetsHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ResponseData<ListModel<Sweet>>> Handle(GetListOfSweets request, CancellationToken cancellationToken)
    {
        try
        {
            // Валидация параметров пагинации
            if (request.pageNo < 1) request = request with { pageNo = 1 };
            if (request.pageSize < 1 || request.pageSize > _maxPageSize)
                request = request with { pageSize = 3 };

            var query = _db.Sweets.AsQueryable();

            // Фильтрация по категории, если указана
            if (!string.IsNullOrEmpty(request.categoryNormalizedName))
            {
                query = query.Where(s => s.Category != null &&
                    s.Category.NormalizedName == request.categoryNormalizedName);
            }

            // Получаем общее количество записей
            var totalCount = await query.CountAsync(cancellationToken);

            // Применяем пагинацию
            var items = await query
                .OrderBy(s => s.Id)
                .Skip((request.pageNo - 1) * request.pageSize)
                .Take(request.pageSize)
                .ToListAsync(cancellationToken);

            var result = new ListModel<Sweet>
            {
                Items = items,
                TotalCount = totalCount,
                PageNo = request.pageNo,
                PageSize = request.pageSize
            };

            return new ResponseData<ListModel<Sweet>> { Data = result };
        }
        catch (Exception ex)
        {
            return new ResponseData<ListModel<Sweet>>
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}