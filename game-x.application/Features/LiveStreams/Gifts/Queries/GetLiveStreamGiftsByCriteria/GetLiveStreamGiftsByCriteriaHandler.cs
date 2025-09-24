using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Features.LiveStreams.Gifts.Queries.GetLiveStreamGiftsByCriteria;

public sealed class GetLiveStreamGiftsByCriteriaHandler : IQueryHandler<GetLiveStreamGiftsByCriteriaQuery, PaginationResult<object>>
{
    public async Task<PaginationResult<object>> Handle(GetLiveStreamGiftsByCriteriaQuery request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return new PaginationResult<object>(
            Array.Empty<object>(),
            0,
            0,
            request.PageIndex,
            request.PageSize);
    }
}
