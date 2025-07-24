namespace game_x.application.Contract.Persistence.Repo;

public interface IMediaFileRepo
{
    Task<MediaFile> FindAsync(int id, CancellationToken ct = default);

    Task<bool> IsExistAsync(int id, CancellationToken ct = default);

    Task AddAsync(MediaFile file, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<MediaFile> files, CancellationToken ct = default);

    Task RemoveAsync(int id, CancellationToken ct = default);
}
