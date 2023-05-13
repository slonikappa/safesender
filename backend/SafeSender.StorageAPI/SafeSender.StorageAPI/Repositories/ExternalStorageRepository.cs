using AnonFilesApi.Interfaces;
using AnonFilesApi.Models;
using SafeSender.StorageAPI.Interfaces;
using SafeSender.StorageAPI.Models;

namespace SafeSender.StorageAPI.Repositories;

public class ExternalStorageRepository : IFilesRepository
{
    private readonly ILogger<ExternalStorageRepository> _logger;
    private readonly IAnonfilesApiClient _apiClient;

    public ExternalStorageRepository(ILogger<ExternalStorageRepository> logger, IAnonfilesApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public async Task<byte[]> GetFileBytes(string fileHash)
    {
        return await _apiClient.DownloadFileByHashAsync(fileHash);
    }

    public async Task<FileSaveInfo> SaveFileBytes(string fileName, byte[] fileData)
    {
        var response = await _apiClient.UploadFileAsync(fileData, fileName);
        return new FileSaveInfo(response.Status, response.Data.File.Metadata.Id);
    }
}