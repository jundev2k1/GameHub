using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class LiveStreamGiftRepo(GameXContext context) : ILiveStreamGiftRepo, IRepository
{
    public async Task<PaginationResult<LiveStreamGift>> GetsByCriteriaAsync(
        Func<IQueryable<LiveStreamGift>, IQueryable<LiveStreamGift>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.LiveStreamGifts
            .AsNoTracking()
            .Include(lsg => lsg.Image)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<LiveStreamGift>(
            result,
            totalCount,
            totalPageCount,
            page,
            pageSize);
    }

    public async Task<LiveStreamGift> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.LiveStreamGifts
            .AsNoTracking()
            .FirstOrDefaultAsync(lsg => lsg.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);
    }

    public async Task CreateAsync(LiveStreamGift gift, CancellationToken ct)
    {
        await context.LiveStreamGifts.AddAsync(gift, ct);
    }

    public async Task UpdateAsync(Guid id, Func<LiveStreamGift, Task> updateAction, CancellationToken ct)
    {
        var targetCategory = await context.LiveStreamGifts
            .FirstOrDefaultAsync(lsg => lsg.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        await updateAction.Invoke(targetCategory);
    }
}
