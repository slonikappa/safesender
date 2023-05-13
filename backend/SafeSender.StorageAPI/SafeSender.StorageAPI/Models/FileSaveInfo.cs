namespace SafeSender.StorageAPI.Models;

public record struct FileSaveInfo(bool Status, string? ExternalToken = null);