using Microsoft.Extensions.Options;
using SafeSender.StorageAPI.Interfaces;
using SafeSender.StorageAPI.Models;
using SafeSender.StorageAPI.Models.ApiModels;
using SafeSender.StorageAPI.Options;

namespace SafeSender.StorageAPI.Services;

/// <summary>
/// Files service
/// </summary>
public class FilesService : IFilesService
{
    private readonly IFilesRepository _filesRepository;
    private readonly IFilesInternalInfosRepository _filesInternalInfosRepository;
    private readonly IOptionsMonitor<StorageOptions> _storageOptions;

    public FilesService(IFilesRepository filesRepository,
        IFilesInternalInfosRepository filesInternalInfosRepository,
        IOptionsMonitor<StorageOptions> storageOptions)
    {
        _filesRepository = filesRepository;
        _filesInternalInfosRepository = filesInternalInfosRepository;
        _storageOptions = storageOptions;
    }

    public async Task<(bool status, string? token)> UploadFile(UploadFileRequestModel model)
    {
        var savingResult = await _filesRepository.SaveFileBytes(model.FileName, model.FileBytes);

        var fileInternalInfo = new FileInternalInfo
        {
            FileName = model.FileName,
            StorageFileIdentifier = savingResult.ExternalToken ?? model.FileName,
            PasswordHash = model.PasswordHash,
            StorageType = _storageOptions.CurrentValue.Type,
        };
       
        await _filesInternalInfosRepository.Add(fileInternalInfo);
        
        return savingResult.Status
            ? (savingResult.Status, fileInternalInfo.Token)
            : (savingResult.Status, null);
    }

    public async Task<DownloadFileResponseModel> DownloadFile(string token)
    {
        var fileInternalInfo = await _filesInternalInfosRepository.GetByToken(token);
        
        if (fileInternalInfo is null)
        {
            throw new FileNotFoundException(
                $"DownloadFile - Information about file by specified token not found. Token: {token}");
        }

        var fileBytes = await _filesRepository.GetFileBytes(fileInternalInfo.StorageFileIdentifier);

        if (!fileBytes.Any())
        {
            throw new FileNotFoundException("File not found by specified token");
        }

        return new()
        {
            FileBytes = fileBytes,
            FileName = fileInternalInfo.FileName,
            PasswordHash = fileInternalInfo.PasswordHash,
        };
    }
}