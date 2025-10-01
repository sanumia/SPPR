using Lab1.API.Data;
using Lab1.Domain.Entities;
using Lab1.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lab1.API.UseCases
{
    // Запрос
    public sealed record GetListOfCategories() : IRequest<ResponseData<ListModel<Category>>>;

    // Обработчик запроса
    public class GetListOfCategoriesHandler : IRequestHandler<GetListOfCategories, ResponseData<ListModel<Category>>>
    {
        private readonly AppDbContext _db;

        public GetListOfCategoriesHandler(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ResponseData<ListModel<Category>>> Handle(GetListOfCategories request, CancellationToken cancellationToken)
        {
            var categories = await _db.Categories.ToListAsync(cancellationToken);

            var listModel = new ListModel<Category>
            {
                Items = categories,
                CurrentPage = 1,
                TotalPages = 1,
                CurrentCategory = null
            };

            return ResponseData<ListModel<Category>>.Success(listModel);
        }
    }
}
