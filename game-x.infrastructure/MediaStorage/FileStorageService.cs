using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.share.Extensions;
using game_x.share.Settings;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace game_x.infrastructure.MediaStorage;

public sealed class FileStorageService(
    IMinioClient client,
    IOptions<MinioSettings> settings,
    IAppLogger<FileStorageService> logger) : IFileStorageService
{
    /// <summary>Default number of objects to delete per batch</summary>
    private const int DefaultChunkSize = 500;

    /// <summary>
    /// Uploads a file stream to the specified bucket and object path.<br />
    /// Ensures the bucket exists before uploading.
    /// </summary>
    public async Task UploadFileAsync(
        Stream fileStream,
        BucketName bucketName,
        ObjectName objectName,
        MimeType mimeType,
        CancellationToken ct = default)
    {
        try
        {
            await EnsureBucketExistsAsync(bucketName, ct);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName.Value)
                .WithObject(objectName.Value)
                .WithStreamData(fileStream)
                .WithContentType(mimeType.Value)
                .WithObjectSize(fileStream.Length);
            await client.PutObjectAsync(putObjectArgs, ct);
        }
        catch (Exception ex)
        {
            logger.LogError("UploadFileAsync failed: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a file stream from the specified bucket and object path.
    /// </summary>
    public async Task<Stream> GetFileAsync(
        BucketName bucketName,
        ObjectName objectName,
        CancellationToken ct = default)
    {
        var memoryStream = new MemoryStream();
        await client.GetObjectAsync(new GetObjectArgs()
            .WithBucket(bucketName.Value)
            .WithObject(objectName.Value)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream)), ct);

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    /// <summary>
    /// Deletes a single file from the specified bucket.
    /// </summary>
    public async Task DeleteFileAsync(
        BucketName bucketName,
        ObjectName objectName,
        CancellationToken ct = default)
    {
        var removeArgs = new RemoveObjectArgs()
            .WithBucket(bucketName.Value)
            .WithObject(objectName.Value);
        await client.RemoveObjectAsync(removeArgs, ct);
    }

    /// <summary>
    /// Deletes all files under a given prefix (folder-like) within the specified bucket.<br />
    /// This simulates folder deletion using prefix filtering.
    /// </summary>
    public async Task DeleteFolderAsync(
        BucketName bucketName,
        string prefix,
        CancellationToken ct = default)
    {
        var deleteFiles = await GetAllFileNamesAsync(bucketName, prefix, ct);
        if (deleteFiles.Length == 0) return;

        // Batch delete it to avoid exceeding API limits
        foreach (var groupFiles in deleteFiles.Chunk(DefaultChunkSize))
        {
            var deleteObjects = groupFiles
                .Select(fileName => new DeleteObject(fileName));
            await client.RemoveObjectsAsync(new RemoveObjectsArgs()
                .WithBucket(bucketName.Value)
                .WithObjects(groupFiles), ct);
        }
    }

    /// <summary>
    /// Generates a presigned URL for downloading a file directly from MinIO<br />
    /// (Frontend can access the file without routing through backend).
    /// </summary>
    public async Task<string> GenerateDownloadUrlAsync(
        BucketName bucketName,
        ObjectName objectName,
        TimeSpan expiry,
        CancellationToken ct = default)
    {
        var urlClient = CreatePublicClient();

        var args = new PresignedGetObjectArgs()
            .WithBucket(bucketName.Value)
            .WithObject(objectName.Value)
            .WithExpiry((int)expiry.TotalSeconds);
        var result = await urlClient.PresignedGetObjectAsync(args);
        return result;
    }
    
    /// <summary>
    /// Generates a presigned URL for uploading a file directly to MinIO<br />
    /// (Frontend can upload the file without routing through backend).
    /// </summary>
    public async Task<string> GenerateUploadUrlAsync(
        BucketName bucketName,
        ObjectName objectName,
        TimeSpan expiry,
        CancellationToken ct = default)
    {
        try
        {
            await EnsureBucketExistsAsync(bucketName, ct);

            var args = new PresignedPutObjectArgs()
                .WithBucket(bucketName.Value)
                .WithObject(objectName.Value)
                .WithExpiry((int)expiry.TotalSeconds);
            var result = await client.PresignedPutObjectAsync(args);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError("GenerateUploadUrlAsync failed: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Creates a new MinIO client using public endpoint configuration<br />
    /// to be used for generating public URLs.
    /// </summary>
    private IMinioClient CreatePublicClient()
    {
        var publicHost = settings.Value.PublicEndpoint;
        var accessKey = settings.Value.AccessKey;
        var secretKey = settings.Value.SecretKey;
        var useSslPublic = settings.Value.UseSslPublic;

        var publicUri = new Uri(publicHost);
        var builder = new MinioClient()
            .WithCredentials(accessKey, secretKey)
            .WithSSL(useSslPublic);
        var publicClient = (useSslPublic && publicUri.Port != 443) || (!useSslPublic && publicUri.Port != 80)
            ? builder.WithEndpoint(publicUri.Host, publicUri.Port).Build()
            : builder.WithEndpoint(publicUri.Host).Build();
        return publicClient;
    }

    /// <summary>
    /// Retrieves all file names (object keys) under the given prefix.<br />
    /// Used for bulk operations like folder deletion.
    /// </summary>
    private async Task<string[]> GetAllFileNamesAsync(
        BucketName bucketName,
        string prefix = "",
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix, nameof(prefix));
        var isExist = await IsExistBucketAsync(bucketName, ct);
        if (!isExist) return [];

        var listArgs = new ListObjectsArgs()
            .WithBucket(bucketName.Value)
            .WithPrefix(prefix)
            .WithRecursive(true);
        var fileNames = new List<string>();
        await foreach (var item in client.ListObjectsEnumAsync(listArgs, ct))
        {
            if (item.Key.IsNullOrEmpty()) continue;

            fileNames.Add(item.Key);
        }

        return [.. fileNames];
    }

    /// <summary>
    /// Checks if a bucket exists in MinIO.
    /// </summary>
    private async Task<bool> IsExistBucketAsync(BucketName bucketName, CancellationToken ct)
        => await client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName.Value), ct);

    /// <summary>
    /// Ensures the bucket exists, creates it if not.
    /// </summary>
    private async Task EnsureBucketExistsAsync(BucketName bucketName, CancellationToken ct = default)
    {
        var isExist = await IsExistBucketAsync(bucketName, ct);
        if (isExist) return;

        await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName.Value), ct);
    }
    
    public async Task<PresignedUploadTicket> CreatePresignedPutAsync(
        BucketName bucket,
        ObjectName objectName,
        MimeType mimeType,
        int sizeBytes,
        TimeSpan expiry,
        CancellationToken ct = default)
    {
        try
        {
            await EnsureBucketExistsAsync(bucket, ct);

            // MinIO has a maximum limit of ~7 days
            var seconds = (int)Math.Clamp(expiry.TotalSeconds, 1, 7 * 24 * 3600);

            var args = new PresignedPutObjectArgs()
                .WithBucket(bucket.Value)
                .WithObject(objectName.Value)
                .WithExpiry(seconds);

            var url = await client.PresignedPutObjectAsync(args).ConfigureAwait(false);

            var headers = new Dictionary<string, string>
            {
                ["Content-Type"] = mimeType.Value
            };

            return new PresignedUploadTicket(url, headers);
        }
        catch (Exception ex)
        {
            logger.LogError("CreatePresignedPutAsync failed: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<StoredObjectInfo?> HeadObjectAsync(
        BucketName bucketName,
        ObjectName objectName,
        CancellationToken ct = default)
    {
        try
        {
            var stat = await client.StatObjectAsync(new StatObjectArgs()
                .WithBucket(bucketName.Value)
                .WithObject(objectName.Value), ct).ConfigureAwait(false);

            return new StoredObjectInfo(
                ContentType: stat.ContentType ?? "application/octet-stream",
                ContentLength: stat.Size,
                ETag: stat.ETag
            );
        }
        catch (Exception)
        {
            return null;
        }
    }
}
