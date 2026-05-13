using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Shared.Interfaces;

namespace Shared.Services;

public class FileService(IWebHostEnvironment environment) : IFileService
{
    private readonly IWebHostEnvironment _environment = environment;

    public async Task<string> SaveFileAsync(IFormFile file, string subDirectory)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is null or empty.");

        var uploadsFolder = Path.Combine(_environment.WebRootPath, subDirectory);
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return uniqueFileName;
    }

    public void DeleteFile(string fileName, string subDirectory)
    {
        if (string.IsNullOrEmpty(fileName)) return;
        var filePath = Path.Combine(_environment.WebRootPath, subDirectory, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}