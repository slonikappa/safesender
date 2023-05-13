using System.ComponentModel.DataAnnotations;

namespace SafeSender.StorageAPI.Models;

/// <summary>
/// Upload file request model
/// </summary>
public sealed class UploadFileRequestModel
{
    /// <summary>
    /// File represented by byte array
    /// </summary>
    [Required]
    public byte[] FileBytes { get; set; } = default!;
    
    /// <summary>
    /// File name with extension (if extension provided)
    /// </summary>
    [Required]
    public string FileName { get; set; } = default!;
    
    /// <summary>
    /// Password hash
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = default!;
}