using SafeSender.StorageAPI.Models;

namespace SafeSender.StorageAPI.Interfaces;

public interface IFilesRepository
{
    Task<byte[]> GetFileBytes(string fileHash);
    Task<FileSaveInfo> SaveFileBytes(string fileName, byte[] fileData);
}