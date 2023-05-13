using SafeSender.StorageAPI.Models;

namespace SafeSender.StorageAPI.Interfaces;

public interface IFilesInternalInfosRepository
{
    Task<IEnumerable<FileInternalInfo>> GetAll(int skip = 0, int limit = 0);
    Task<FileInternalInfo?> GetByToken(string token);
    Task Add(FileInternalInfo fileInternalInfo);
    
    // TODO: Delete, Update
}