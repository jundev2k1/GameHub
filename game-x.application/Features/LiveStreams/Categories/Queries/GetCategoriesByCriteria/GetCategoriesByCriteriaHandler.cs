using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Categories.Dtos;
using game_x.application.Features.LiveStreams.Categories.Mapping;

namespace game_x.application.Features.LiveStreams.Categories.Queries.GetCategoriesByCriteria;

public sealed class GetCategoriesByCriteriaHandler(
    ICriteriaBuilder<LiveStreamCategory> builder,
    ILiveStreamCategoryRepo liveStreamCategoryRepo) : IQueryHandler<GetCategoriesByCriteriaQuery, PaginationResult<LiveStreamCategoryListItemDto>>
{
    public async Task<PaginationResult<LiveStreamCategoryListItemDto>> Handle(
        GetCategoriesByCriteriaQuery request,
        CancellationToken ct = default)
    {
        var items = await liveStreamCategoryRepo.GetsByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword => lsc => lsc.Name.ToLowerInvariant().Contains(keyword.ToLowerInvariant())),
            request.PageIndex,
            request.PageSize,
            ct);
        return items.ToSearchResult();
    }
}
