using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.Games.Mapping;

namespace game_x.application.Features.Games.Admin.Queries.GetGamesByCriteria;

public sealed class GetGamesByCriteriaHandler(
    ICriteriaBuilder<Game> builder,
    IGameRepo gameRepo) : IQueryHandler<GetGamesByCriteriaQuery, PaginationResult<GetGamesByCriteriaListItem>>
{
    public async Task<PaginationResult<GetGamesByCriteriaListItem>> Handle(GetGamesByCriteriaQuery request, CancellationToken ct = default)
    {
        var result = await gameRepo.GetsByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword => entity => entity.Name.ToLowerInvariant().Contains(keyword.ToLowerInvariant()),
                GameFilterExtensions.Options),
            request.PageIndex,
            request.PageSize,
            ct);
        return result.ToSearchResult();
    }
}
