using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class MediaFileRepo(GameXContext context) : IMediaFileRepo, IRepository
{
    public async Task<MediaFile> FindAsync(int id, CancellationToken ct = default)
    {
        return await context.MediaFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(mf => mf.Id == id, ct)
            ?? throw new NotFoundException("Media file not found.");
    }

    public async Task<bool> IsExistAsync(int id, CancellationToken ct = default)
        => await context.MediaFiles.AsNoTracking().AnyAsync(mf => mf.Id == id, ct);

    public async Task AddAsync(MediaFile file, CancellationToken ct = default)
        => await context.MediaFiles.AddAsync(file, ct);

    public async Task AddRangeAsync(IEnumerable<MediaFile> files, CancellationToken ct = default)
        => await context.MediaFiles.AddRangeAsync(files, ct);

    public async Task RemoveAsync(int id, CancellationToken ct = default)
    {
        var mediaFile = await context.MediaFiles
            .FirstOrDefaultAsync(mf => mf.Id == id, ct)
            ?? throw new NotFoundException("Media file not found.");

        context.MediaFiles.Remove(mediaFile);
    }
}