using System.Diagnostics;
using SafeSender.StorageAPI.Interfaces;
using SafeSender.StorageAPI.Models;

namespace SafeSender.StorageAPI.Repositories;

public class LocalStorageRepository : IFilesRepository
{
    private const string FileStoragePath = ".";
    private readonly ILogger<LocalStorageRepository> _logger;

    public LocalStorageRepository(ILogger<LocalStorageRepository> logger)
    {
        _logger = logger;
    }

    public async Task<byte[]> GetFileBytes(string fileName)
    {
        var fullPath = GetFullPath(fileName);
        return await File.ReadAllBytesAsync(fullPath);
    }

    public Task<FileSaveInfo> SaveFileBytes(string fileName, byte[] fileData)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("fileName cannot be null or empty.", nameof(fileName));
        }
        
        try
        {
            File.WriteAllBytes(GetFullPath(fileName), fileData);
            
            return Task.FromResult(new FileSaveInfo(true));
        }
        catch (Exception e)
        {
            Console.WriteLine($"File saving failed. Message: {e.Message}");
            _logger.LogError(
                "{ClassName} - File saving failed. Message: {Message}. Stack trace: {StackTrace}. Inner exception: {InnerException}", 
                nameof(LocalStorageRepository), e.Message, e.StackTrace, e.InnerException);
            
            throw;
        }
    }

    private string GetFullPath(string fileName)
    {
        return FileStoragePath + Path.DirectorySeparatorChar + fileName;
    }
}