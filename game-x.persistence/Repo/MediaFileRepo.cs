using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class MediaFileRepo(GameXContext context) : IMediaFileRepo
{
    public async Task<MediaFile> FindAsync(int id, CancellationToken ct = default)
    {
        return await context.MediaFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(mf => mf.Id == id, ct)
            ?? throw new NotFoundException("Media file not found.");
    }

    public async Task<MediaFile> FindPassportAsync(string userId, CancellationToken ct = default)
    {
        return await context.MediaFiles
            .AsNoTracking()
            .Join(context.UserPassport.Where(up => up.AppUserId == userId),
                mf => mf.Id,
                up => up.PassportImageId,
                (file, user) => file)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("Passport not found.");
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
