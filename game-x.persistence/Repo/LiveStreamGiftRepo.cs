using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.LiveStreams.Gifts.Dtos;
using Mapster;

namespace game_x.persistence.Repo;

public sealed class LiveStreamGiftRepo(
    GameXContext context,
    IFileManagerCacheService fileManager) : ILiveStreamGiftRepo, IRepository
{
    public async Task<PaginationResult<LiveStreamGift>> GetsByCriteriaAsync(
        Func<IQueryable<LiveStreamGift>, IQueryable<LiveStreamGift>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.LiveStreamGifts
            .AsNoTracking()
            .Where(lsg => lsg.IsDeleted == false)
            .Include(lsg => lsg.Icon)
            .Include(lsg => lsg.Animation)
            .Include(lsg => lsg.GiftPrices)
            .ThenInclude(gp => gp.CryptoToken)
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

    public async Task<LiveStreamGift[]> GetAllActivesAsync(CancellationToken ct = default)
    {
        return await context.LiveStreamGifts
            .AsNoTracking()
            .AsSplitQuery()
            .Where(lsg => lsg.IsDeleted == false && lsg.IsActive)
            .Include(lsg => lsg.Icon)
            .Include(lsg => lsg.Animation)
            .Include(lsg => lsg.GiftPrices)
            .ThenInclude(gp => gp.CryptoToken)
            .ToArrayAsync(ct);
    }

    public async Task<LiveStreamGift> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.LiveStreamGifts
            .AsNoTracking()
            .AsSplitQuery()
            .Include(lsg => lsg.Icon)
            .Include(lsg => lsg.Animation)
            .Include(lsg => lsg.GiftPrices)
            .ThenInclude(gp => gp.CryptoToken)
            .FirstOrDefaultAsync(lsg => lsg.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);
    }

    public async Task<LiveStreamGiftDetailDto> GetDetailByIdAsync(Guid id, CancellationToken ct = default)
    {
        var target = await context.LiveStreamGifts
            .AsNoTracking()
            .AsSplitQuery()
            .Include(lsg => lsg.Icon)
            .Include(lsg => lsg.Animation)
            .Include(lsg => lsg.GiftPrices)
            .ThenInclude(gp => gp.CryptoToken)
            .FirstOrDefaultAsync(g => g.PublicId == id && g.IsDeleted == false, ct)
            ?? throw new NotFoundException(nameof(id), id);

        var dto = target.Adapt<LiveStreamGiftDetailDto>();
        if (target.Icon != null)
        {
            var imageInfo = await fileManager.GetFileInfo(target.Icon, ct);
            dto.IconUrl = imageInfo?.Url;
        }
        if (target.Animation != null)
        {
            var imageInfo = await fileManager.GetFileInfo(target.Animation, ct);
            dto.AnimationUrl = imageInfo?.Url;
        }
        return dto;
    }

    public async Task CreateAsync(LiveStreamGift gift, CancellationToken ct)
    {
        await context.LiveStreamGifts.AddAsync(gift, ct);
    }

    public async Task UpdateAsync(Guid id, Func<LiveStreamGift, Task> updateAction, CancellationToken ct)
    {
        var targetGift = await context.LiveStreamGifts
            .Include(lsg => lsg.Icon)
            .Include(lsg => lsg.Animation)
            .Include(lsg => lsg.GiftPrices)
            .ThenInclude(gp => gp.CryptoToken)
            .FirstOrDefaultAsync(lsg => lsg.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        await updateAction.Invoke(targetGift);
    }
}
