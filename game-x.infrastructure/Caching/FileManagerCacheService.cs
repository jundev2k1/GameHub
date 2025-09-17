using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Dto;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;

namespace game_x.infrastructure.Caching;

public sealed class FileManagerCacheService(
    IMemoryCache cache,
    IMediaFileRepo mediaFileRepo,
    IFileStorageService storageService) : CacheService(cache), IFileManagerCacheService
{
    private const string CacheKeyPrefix = "fileManager:";

    public async Task RefreshImage(MediaFile file, TimeSpan? ExpiredTime = null, CancellationToken ct = default)
    {
        var url = await storageService.GenerateDownloadUrlAsync(
            file.BucketName,
            file.ObjectName,
            ExpiredTime ?? TimeSpan.FromHours(24),
            ct);

        var cacheKey = $"{CacheKeyPrefix}{file.Id}";
        var fileInfo = new MediaFileInfo(file.Id, file.FileName, url);
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ExpiredTime ?? TimeSpan.FromHours(24)
        };
        Set(cacheKey, fileInfo, options);
    }
    public async Task RefreshImage(int fileId, TimeSpan? ExpiredTime = null, CancellationToken ct = default)
    {
        var file = await mediaFileRepo.FindAsync(fileId, ct);
        var url = await storageService.GenerateDownloadUrlAsync(
            file.BucketName,
            file.ObjectName,
            ExpiredTime ?? TimeSpan.FromHours(24),
            ct);

        var cacheKey = $"{CacheKeyPrefix}{file.Id}";
        var fileInfo = new MediaFileInfo(file.Id, file.FileName, url);
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ExpiredTime ?? TimeSpan.FromHours(24)
        };
        Set(cacheKey, fileInfo, options);
    }

    public async Task<MediaFileInfo?> GetImageUrl(MediaFile file, CancellationToken ct = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{file.Id}";
        var fileInfo = Get<MediaFileInfo?>(cacheKey);
        if (fileInfo is null) await RefreshImage(file, null, ct);

        return Get<MediaFileInfo?>(cacheKey);
    }
    public async Task<MediaFileInfo?> GetImageUrl(int fileId, CancellationToken ct = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{fileId}";
        var file = Get<MediaFileInfo?>(cacheKey);
        if (file is null) await RefreshImage(fileId, null, ct);

        return Get<MediaFileInfo?>(cacheKey);
    }

    public void RemoveImage(int fileId)
    {
        var cacheKey = $"{CacheKeyPrefix}{fileId}";
        Remove(cacheKey);
    }
}
