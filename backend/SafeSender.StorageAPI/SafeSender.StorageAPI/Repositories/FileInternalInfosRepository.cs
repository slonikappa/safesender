using MongoDB.Driver;
using SafeSender.StorageAPI.Database;
using SafeSender.StorageAPI.Interfaces;
using SafeSender.StorageAPI.Models;

namespace SafeSender.StorageAPI.Repositories;

public class FileInternalInfosRepository : IFilesInternalInfosRepository
{
    private readonly IMongoDbContext _context;

    public FileInternalInfosRepository(IMongoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FileInternalInfo>> GetAll(int skip = 0, int limit = 0)
    {
        var options = new FindOptions<FileInternalInfo>();
        if (skip > 0)
        {
            options.Skip = skip;
        }
        if (limit > 0)
        {
            options.Limit = limit;
        }
        
        var entities = await _context.FilesInternalInfos
            .FindAsync(_ => true, options);

        return entities.ToList();
    }

    public async Task<FileInternalInfo?> GetByToken(string token)
    {
        var entity = await _context.FilesInternalInfos
            .FindAsync(f => f.Token == token);

        return await entity.FirstOrDefaultAsync();
    }

    public async Task Add(FileInternalInfo fileInternalInfo)
    {
        if (fileInternalInfo is null)
        {
            throw new ArgumentNullException(nameof(fileInternalInfo), "Add item - File internal info cannot be null");
        }
        
        await _context.FilesInternalInfos.InsertOneAsync(fileInternalInfo);
    }
}