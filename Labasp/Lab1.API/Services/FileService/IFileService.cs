namespace Lab1.API.Services.FileService
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
    }
}
