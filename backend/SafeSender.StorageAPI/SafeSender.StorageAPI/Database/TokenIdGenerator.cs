using Base62;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

namespace SafeSender.StorageAPI.Database;

public class TokenIdGenerator : IIdGenerator
{
    public object GenerateId(object container, object document)
    {
        return new CombGuidGenerator()
            .NewCombGuid(Guid.NewGuid(), DateTime.Now.ToUniversalTime())
            .ToByteArray()
            .ToBase62(true);
    }

    public bool IsEmpty(object? id)
    {
        return id is null || id.ToString() == string.Empty;
    }
}