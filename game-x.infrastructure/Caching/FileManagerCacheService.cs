using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Dto;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class FileManagerCacheService(
    IMemoryCache cache,
    IMediaFileRepo mediaFileRepo,
    IFileStorageService storageService) : CacheService(cache), IFileManagerCacheService
{
    private const string CacheKeyPrefix = "fileManager:";
    private readonly TimeSpan _defaultExpiredTime = TimeSpan.FromHours(24);

    public async Task RefreshImage(MediaFile file, TimeSpan? expiredTime = null, CancellationToken ct = default)
    {
        var url = await storageService.GenerateDownloadUrlAsync(
            file.BucketName,
            file.ObjectName,
            expiredTime ?? _defaultExpiredTime,
            ct);

        var cacheKey = $"{CacheKeyPrefix}{file.Id}";
        var fileInfo = new MediaFileInfo(file.Id, file.FileName, url);
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiredTime ?? _defaultExpiredTime
        };
        Set(cacheKey, fileInfo, options);
    }
    public async Task RefreshImage(int fileId, TimeSpan? expiredTime = null, CancellationToken ct = default)
    {
        var file = await mediaFileRepo.FindAsync(fileId, ct);
        var url = await storageService.GenerateDownloadUrlAsync(
            file.BucketName,
            file.ObjectName,
            expiredTime ?? _defaultExpiredTime,
            ct);

        var cacheKey = $"{CacheKeyPrefix}{file.Id}";
        var fileInfo = new MediaFileInfo(file.Id, file.FileName, url);
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiredTime ?? _defaultExpiredTime
        };
        Set(cacheKey, fileInfo, options);
    }

    public async Task<MediaFileInfo?> GetFileInfo(MediaFile file, CancellationToken ct = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{file.Id}";
        var fileInfo = Get<MediaFileInfo?>(cacheKey);
        if (fileInfo is null) await RefreshImage(file, null, ct);

        return Get<MediaFileInfo?>(cacheKey);
    }
    public async Task<MediaFileInfo?> GetFileInfo(int fileId, CancellationToken ct = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{fileId}";
        var file = Get<MediaFileInfo?>(cacheKey);
        if (file is null) await RefreshImage(fileId, null, ct);

        return Get<MediaFileInfo?>(cacheKey);
    }

    public async Task<string?> GetFileUrl(MediaFile? file, CancellationToken ct = default)
    {
        if (file is null) return null;

        var fileInfo = await GetFileInfo(file, ct);
        return fileInfo?.Url;
    }
    public async Task<string?> GetFileUrl(int? fileId, CancellationToken ct = default)
    {
        if (!fileId.HasValue) return null;

        var fileInfo = await GetFileInfo(fileId.Value, ct);
        return fileInfo?.Url;
    }

    public void RemoveImage(int fileId)
    {
        var cacheKey = $"{CacheKeyPrefix}{fileId}";
        Remove(cacheKey);
    }
}