namespace SafeSender.StorageAPI.Extensions;

public static class StringExtensions
{
    public static string? NullIfEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str) ? null : str;
    }
}