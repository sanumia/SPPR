using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Lab1.API.Services.FileService
{
    public class LocalFileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public LocalFileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "/Images/default-profile-picture.png";
            }

            // 🟢 Папка avatars
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "Images", "avatars");

            // Создаём директорию если нет
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Генерируем уникальное имя файла
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Сохраняем
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 🟢 Возвращаем корректный URL
            return $"/Images/avatars/{fileName}";
        }
    }
}
