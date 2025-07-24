namespace game_x.application.Contract.Infrastructure.FileStorage;

public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file stream to the specified bucket and object path.<br />
    /// Ensures the bucket exists before uploading.
    /// </summary>
    Task UploadFileAsync(
        Stream fileStream,
        BucketName bucketName,
        ObjectName objectName,
        MimeType mimeType,
        CancellationToken ct = default);

    /// <summary>
    /// Retrieves a file stream from the specified bucket and object path.
    /// </summary>
    Task<Stream> GetFileAsync(
        BucketName bucketName,
        ObjectName objectName,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes a single file from the specified bucket.
    /// </summary>
    Task DeleteFileAsync(
        BucketName bucketName,
        ObjectName objectName,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes all files under a given prefix (folder-like) within the specified bucket.<br />
    /// This simulates folder deletion using prefix filtering.
    /// </summary>
    Task DeleteFolderAsync(
        BucketName bucketName,
        string prefix,
        CancellationToken ct = default);

    /// <summary>
    /// Generates a presigned URL for downloading a file directly from MinIO<br />
    /// (Frontend can access the file without routing through backend).
    /// </summary>
    Task<string> GenerateDownloadUrlAsync(
        BucketName bucketName,
        ObjectName objectName,
        TimeSpan expiry,
        CancellationToken ct = default);

    /// <summary>
    /// Generates a presigned URL for uploading a file directly to MinIO<br />
    /// (Frontend can upload the file without routing through backend).
    /// </summary>
    Task<string> GenerateUploadUrlAsync(
        BucketName bucketName,
        ObjectName objectName,
        TimeSpan expiry,
        CancellationToken ct = default);
}
