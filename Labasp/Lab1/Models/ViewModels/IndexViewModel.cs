using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab1.Models.ViewModels
{
    public class IndexViewModel
    {
    public string LabWorkTitle { get; set; }
    public int? SelectedItemId { get; set; }
    public SelectList SelectItems { get; set; }
    
    // Конструктор для удобства
    public IndexViewModel()
    {
        var items = new List<ListDemo>
        {
            new ListDemo { Id = 1, Name = "Первый элемент" },
            new ListDemo { Id = 2, Name = "Второй элемент" },
            new ListDemo { Id = 3, Name = "Третий элемент" },
            new ListDemo { Id = 4, Name = "Четвертый элемент" },
            new ListDemo { Id = 5, Name = "Пятый элемент" }
        };
        
        SelectItems = new SelectList(items, "Id", "Name");
        LabWorkTitle = "Лабораторная работа №2";
    }
    }
}