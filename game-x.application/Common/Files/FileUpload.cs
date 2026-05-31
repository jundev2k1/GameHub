using Microsoft.AspNetCore.Http;

namespace game_x.application.Common.Files;

public sealed class FileUpload
{
    /// <summary>Supported file extensions for image uploads.</summary>
    public static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];

    /// <summary>Supported MIME types for image uploads.</summary>
    public static readonly string[] ImageMimeTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"];

    /// <summary>Supported file extensions for video uploads.</summary>
    public static readonly string[] VideoExtensions = [".mp4", ".mkv", ".avi", ".mov"];

    /// <summary>Supported MIME types for video uploads.</summary>
    public static readonly string[] VideoMimeTypes = ["video/mp4", "video/x-matroska", "video/x-msvideo", "video/quicktime"];

    /// <summary>Maximum allowed size for image uploads in bytes (10 MB).</summary>
    public const int ImageMaxSize = 10 * 1024 * 1024;

    /// <summary>Maximum allowed size for video uploads in bytes (100 MB).</summary>
    public const int VideoMaxSize = 100 * 1024 * 1024;

    /// <summary>Combined list of all allowed file extensions for both images and videos.</summary>
    private static readonly string[] AllowedExtensions = [.. ImageExtensions, .. VideoExtensions];

    /// <summary>Combined list of all allowed MIME types for both images and videos.</summary>
    private static readonly string[] AllowedMimeTypes = [.. ImageMimeTypes, .. VideoMimeTypes];

    /// <summary>Absolute maximum allowed file size in bytes for any single upload (100 MB).</summary>
    private const int MaxFileSize = 100 * 1024 * 1024;

    public Stream Content { get; init; } = default!;
    public string FileName { get; init; } = default!;
    public string Extension { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public long Length { get; init; }

    public static FileUpload FromFormFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.");

        // Validate file properties
        ValidateFile(file.FileName, file.ContentType, file.Length);

        return new FileUpload
        {
            Content = file.OpenReadStream(),
            FileName = file.FileName,
            Extension = Path.GetExtension(file.FileName),
            ContentType = file.ContentType,
            Length = file.Length
        };
    }

    public static FileUpload FromStream(Stream stream, string fileName, string contentType, long contentLength)
    {
        if (stream == null)
            throw new ArgumentException("Stream is empty.");

        if (contentLength <= 0)
            throw new ArgumentException("Content length must be greater than zero.");

        // Validate file properties
        ValidateFile(fileName, contentType, contentLength);

        return new FileUpload
        {
            Content = stream,
            FileName = fileName,
            Extension = Path.GetExtension(fileName),
            ContentType = contentType,
            Length = contentLength
        };
    }

    /// <summary>
    /// Validates file extension, size and MIME type.
    /// Throws ArgumentException if invalid.
    /// </summary>
    private static void ValidateFile(string fileName, string contentType, long fileSize)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException($"Unsupported file extension: {extension}");

        if (!AllowedMimeTypes.Contains(contentType))
            throw new ArgumentException($"Unsupported MIME type: {contentType}");

        if (fileSize > MaxFileSize)
            throw new ArgumentException($"File size exceeds maximum allowed limit of {MaxFileSize / 1024 / 1024} MB.");
    }
}
