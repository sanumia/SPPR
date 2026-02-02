using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Lab1.API.UseCases
{
    public sealed record SaveImage(IFormFile file) : IRequest<string>;

    public class SaveImageHandler : IRequestHandler<SaveImage, string>
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SaveImageHandler(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Handle(SaveImage request, CancellationToken cancellationToken)
        {
            if (request.file == null || request.file.Length == 0)
            {
                throw new InvalidOperationException("Empty file");
            }

            var imagesFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot"), "images");
            Directory.CreateDirectory(imagesFolder);

            var ext = Path.GetExtension(request.file.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var physicalPath = Path.Combine(imagesFolder, fileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await request.file.CopyToAsync(stream, cancellationToken);
            }

            var requestHost = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = requestHost != null
                ? $"{requestHost.Scheme}://{requestHost.Host}"
                : string.Empty;

            var url = $"{baseUrl}/images/{fileName}";
            return url;
        }
    }
}


