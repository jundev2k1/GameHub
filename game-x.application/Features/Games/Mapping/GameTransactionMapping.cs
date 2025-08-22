using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Mapping;

public static class GameTransactionMapping
{
    public static PaginationResult<GameTransactionDto> ToSearchResult(this PaginationResult<GameTransaction> data)
    {
        var result = new PaginationResult<GameTransactionDto>(
            items: [.. data.Items.Adapt<IEnumerable<GameTransactionDto>>()],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}