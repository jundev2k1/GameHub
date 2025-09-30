using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Gifts.Dtos;
using game_x.application.Features.LiveStreams.Gifts.Mapping;

namespace game_x.application.Features.LiveStreams.Gifts.Queries.GetLiveStreamGiftsByCriteria;

public sealed class GetLiveStreamGiftsByCriteriaHandler(
    ICriteriaBuilder<LiveStreamGift> builder,
    ILiveStreamGiftRepo liveStreamGiftRepo,
    IFileManagerCacheService fileManagerCache) : IQueryHandler<GetLiveStreamGiftsByCriteriaQuery, PaginationResult<LiveStreamGiftDto>>
{
    public async Task<PaginationResult<LiveStreamGiftDto>> Handle(GetLiveStreamGiftsByCriteriaQuery request, CancellationToken ct = default)
    {
        var result = await liveStreamGiftRepo.GetsByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword => gift => gift.Name.ToLower().Contains(keyword.ToLower())),
            request.PageIndex,
            request.PageSize,
            ct);
        Task<(Guid PublicId, string? Icon)>[] giftFileTasks = [.. result.Items
            .Select(async i =>
            {
                var iconImage = await fileManagerCache.GetFileUrl(i.Icon);
                return (i.PublicId, iconImage);
            })];
        var giftFiles = await Task.WhenAll(giftFileTasks);
        return result.ToSearchResult(giftFiles);
    }
}
