using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface ILiveStreamGiftRepo
{
    Task<PaginationResult<LiveStreamGift>> GetsByCriteriaAsync(
        Func<IQueryable<LiveStreamGift>, IQueryable<LiveStreamGift>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<LiveStreamGift> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task CreateAsync(LiveStreamGift gift, CancellationToken ct = default);

    Task UpdateAsync(
        Guid giftId,
        Func<LiveStreamGift, Task> updateAction,
        CancellationToken ct = default);
}
