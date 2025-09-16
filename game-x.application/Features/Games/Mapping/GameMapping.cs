using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Games.Admin.Queries.GetGamesByCriteria;

namespace game_x.application.Features.Games.Mapping;

public static class GameMapping
{
    public static PaginationResult<GetGamesByCriteriaListItem> ToSearchResult(this PaginationResult<Game> data)
    {
        var result = new PaginationResult<GetGamesByCriteriaListItem>(
            items: [.. data.Items.Select(i => i.Adapt<GetGamesByCriteriaListItem>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages, 
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}