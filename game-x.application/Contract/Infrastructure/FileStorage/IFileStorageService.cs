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
        TimeSpan? expiry = null,
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
    
    /// <summary>
    /// Generates a short-lived presigned <c>PUT</c> URL so the frontend can upload
    /// directly to object storage (no backend streaming).
    /// <list type="bullet">
    ///   <item><description>Does not create the object; it is created when the client uploads.</description></item>
    ///   <item><description>If expired, request a new ticket.</description></item>
    ///   <item><description>Client must send the same Content-Type and any returned headers.</description></item>
    /// </list>
    /// </summary>
    Task<PresignedUploadTicket> CreatePresignedPutAsync(
        BucketName bucket, ObjectName objectName, MimeType mimeType, int sizeBytes, TimeSpan expiry, CancellationToken ct);
    
    /// <summary>
    /// <list type="bullet">
    ///   <item><description> Read object metadata without downloading the file (a HEAD request).</description></item>
    ///   <item><description>bucket + object key.</description></item>
    ///   <item><description>Client must send the same Content-Type and any returned headers.</description></item>
    /// </list>
    /// Notes: Use this right after upload to validate size/MIME in the “finalize”
    /// or anytime you need to check existence/metadata cheaply.
    /// </summary>
    Task<StoredObjectInfo?> HeadObjectAsync(
        BucketName bucketName,
        ObjectName objectName,
        CancellationToken ct = default);
}

public sealed record PresignedUploadTicket(string UploadUrl, IReadOnlyDictionary<string,string>? Headers);
public sealed record StoredObjectInfo(string ContentType, long ContentLength, string? ETag);