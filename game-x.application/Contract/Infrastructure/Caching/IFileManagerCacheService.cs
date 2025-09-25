using game_x.application.Contract.Infrastructure.Dto;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IFileManagerCacheService
{
    Task RefreshImage(MediaFile file, TimeSpan? ExpiredTime = null, CancellationToken ct = default);
    Task RefreshImage(int fileId, TimeSpan? ExpiredTime = null, CancellationToken ct = default);

    Task<MediaFileInfo?> GetFileUrl(MediaFile file, CancellationToken ct = default);
    Task<MediaFileInfo?> GetFileUrl(int fileId, CancellationToken ct = default);

    void RemoveImage(int fileId);
}
