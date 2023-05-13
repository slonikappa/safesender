using SafeSender.StorageAPI.Models.Enums;

namespace SafeSender.StorageAPI.Options;

public class StorageOptions
{
    public static string SettingsPath { get; } = "Application:StorageOptions";
    
    public StorageType Type { get; set; }
}