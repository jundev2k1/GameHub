using Microsoft.AspNetCore.Http;

namespace game_x.application.Common.Files;

public sealed class FileUpload
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    private static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"];
    private const int MaxFileSize = 10 * 1024 * 1024; // 10 MB

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

    public static FileUpload FromStream(Stream stream, string fileName, string contentType)
    {
        if (stream == null || stream.Length == 0)
            throw new ArgumentException("Stream is empty.");

        // Validate file properties
        ValidateFile(fileName, contentType, stream.Length);

        return new FileUpload
        {
            Content = stream,
            FileName = fileName,
            ContentType = contentType,
            Length = stream.Length
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
