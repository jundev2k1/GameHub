using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class LiveStreamDonationRepo(GameXContext context) : ILiveStreamDonationRepo, IRepository
{
    public async Task<PaginationResult<LiveStreamDonation>> GetsByCriteriaAsync(
        Func<IQueryable<LiveStreamDonation>, IQueryable<LiveStreamDonation>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.LiveStreamDonations
            .AsNoTracking()
            .Include(lsd => lsd.LivestreamSchedule)
            .Include(lsd => lsd.Donor)
            .Include(lsd => lsd.Gift)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<LiveStreamDonation>(
            result,
            totalCount,
            totalPageCount,
            page,
            pageSize);
    }

    public async Task<LiveStreamDonation> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.LiveStreamDonations
            .AsNoTracking()
            .FirstOrDefaultAsync(lsc => lsc.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);
    }

    public async Task CreateAsync(LiveStreamDonation category, CancellationToken ct)
    {
        await context.LiveStreamDonations.AddAsync(category, ct);
    }

    public async Task UpdateAsync(Guid categoryId, Func<LiveStreamDonation, Task> updateAction, CancellationToken ct)
    {
        var targetCategory = await context.LiveStreamDonations
            .FirstOrDefaultAsync(lsc => lsc.PublicId == categoryId, ct)
            ?? throw new NotFoundException(nameof(categoryId), categoryId);

        await updateAction.Invoke(targetCategory);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var targetCategory = await context.LiveStreamCategories
            .FirstOrDefaultAsync(lsc => lsc.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        context.LiveStreamCategories.Remove(targetCategory);
    }
}
