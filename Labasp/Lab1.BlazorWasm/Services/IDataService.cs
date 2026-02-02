using Lab1.Domain.Entities;

namespace Lab1.BlazorWasm.Services
{
    public interface IDataService
    {
        // Событие, генерируемое при изменении данных
        event Action? DataLoaded;

        // Список категорий
        List<Category> Categories { get; set; }

        // Список сладостей
        List<Sweet> Sweets { get; set; }

        // Признак успешного ответа на запрос к Api
        bool Success { get; set; }

        // Сообщение об ошибке
        string ErrorMessage { get; set; }

        // Количество страниц списка
        int TotalPages { get; set; }

        // Номер текущей страницы
        int CurrentPage { get; set; }

        // Фильтр по категории
        Category? SelectedCategory { get; set; }

        /// <summary>
        /// Получение списка всех сладостей
        /// </summary>
        /// <param name="pageNo">номер страницы списка</param>
        /// <returns></returns>
        public Task GetSweetListAsync(int pageNo = 1);

        /// <summary>
        /// Получение списка категорий
        /// </summary>
        /// <returns></returns>
        public Task GetCategoryListAsync();
    }
}