using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SafeSender.StorageAPI.Models;
using SafeSender.StorageAPI.Options;

namespace SafeSender.StorageAPI.Database;

public class MongoDbContext : IMongoDbContext
{
    public IMongoCollection<FileInternalInfo> FilesInternalInfos { get; init; }
    
    public MongoDbContext(IOptionsMonitor<DatabaseOptions> databaseOptions)
    {
        var url = new MongoUrlBuilder
        {
            Server = new MongoServerAddress(databaseOptions.CurrentValue.Host),
            Username = databaseOptions.CurrentValue.User != string.Empty ? databaseOptions.CurrentValue.User : null,
            Password = databaseOptions.CurrentValue.Password != string.Empty ? databaseOptions.CurrentValue.Password : null,
        }.ToMongoUrl();
        
        var client = new MongoClient(url);
        
        var database = client.GetDatabase("safe-sender-storage");
        
        // Collections initialization
        FilesInternalInfos = database.GetCollection<FileInternalInfo>("FilesInternalInfos");
    }
}