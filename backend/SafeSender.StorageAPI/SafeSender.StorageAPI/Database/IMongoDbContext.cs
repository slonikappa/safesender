using MongoDB.Driver;
using SafeSender.StorageAPI.Models;

namespace SafeSender.StorageAPI.Database;

public interface IMongoDbContext
{
    public IMongoCollection<FileInternalInfo> FilesInternalInfos { get; init; }
}