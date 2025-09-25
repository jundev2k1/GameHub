using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.LiveStreams.Gifts.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface ILiveStreamGiftRepo
{
    Task<PaginationResult<LiveStreamGift>> GetsByCriteriaAsync(
        Func<IQueryable<LiveStreamGift>, IQueryable<LiveStreamGift>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<LiveStreamGift[]> GetAllActivesAsync(CancellationToken ct = default);

    Task<LiveStreamGift> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<LiveStreamGiftDetailDto> GetDetailByIdAsync(Guid id, CancellationToken ct = default);

    Task CreateAsync(LiveStreamGift gift, CancellationToken ct = default);

    Task UpdateAsync(
        Guid giftId,
        Func<LiveStreamGift, Task> updateAction,
        CancellationToken ct = default);
}
