using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lab1.Domain.Models
{
    public class ListModel<T>
    {
        /// <summary>
        /// Коллекция элементов текущей страницы.
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Общее количество элементов с учётом фильтрации.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Размер страницы (число элементов на странице).
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Текущая страница (из JSON поля pageNo).
        /// </summary>
        [JsonPropertyName("pageNo")]
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Общее количество страниц.
        /// </summary>
        public int TotalPages { get; set; } = 1;

        /// <summary>
        /// Нормализованное имя активной категории (если фильтрация применена).
        /// </summary>
        public string? CurrentCategory { get; set; }
    }
}