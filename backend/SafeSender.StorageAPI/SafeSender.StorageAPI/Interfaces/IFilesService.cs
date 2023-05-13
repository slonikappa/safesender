using SafeSender.StorageAPI.Models;
using SafeSender.StorageAPI.Models.ApiModels;

namespace SafeSender.StorageAPI.Interfaces;

public interface IFilesService
{
    /// <summary>
    /// Upload file
    /// </summary>
    /// <param name="model">Upload file request model<see cref="UploadFileRequestModel"/></param>
    /// <returns>Saving status and internal token</returns>
    Task<(bool status, string? token)> UploadFile(UploadFileRequestModel model);
    
    /// <summary>
    /// Download file 
    /// </summary>
    /// <param name="token">Internal token</param>
    /// <returns>Download file response model <see cref="DownloadFileResponseModel"/></returns>
    Task<DownloadFileResponseModel> DownloadFile(string token);
}