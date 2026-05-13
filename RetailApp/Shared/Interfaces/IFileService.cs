using Microsoft.AspNetCore.Http;

namespace Shared.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, string subDirectory);
    void DeleteFile(string fileName, string subDirectory);
}