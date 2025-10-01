using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lab1.Models;
using Lab1.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Создаем модель и передаем в представление
        var model = new IndexViewModel();
        return View(model);
    }
    
    [HttpPost]
    public IActionResult Index(IndexViewModel model)
    {
        // Здесь model.SelectedItemId содержит Id выбранного элемента
        if (model.SelectedItemId.HasValue)
        {
            // Добавляем сообщение о выборе
            ViewData["SelectionMessage"] = $"Выбран элемент с ID: {model.SelectedItemId}";
        }
        
        // Обновляем SelectList для повторного отображения
        var items = new List<ListDemo>
        {
            new ListDemo { Id = 1, Name = "Первый элемент" },
            new ListDemo { Id = 2, Name = "Второй элемент" },
            new ListDemo { Id = 3, Name = "Третий элемент" },
            new ListDemo { Id = 4, Name = "Четвертый элемент" },
            new ListDemo { Id = 5, Name = "Пятый элемент" }
        };
        
        model.SelectItems = new SelectList(items, "Id", "Name");
        ViewData["LabWorkTitle"] = "Лабораторная работа №2";
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
