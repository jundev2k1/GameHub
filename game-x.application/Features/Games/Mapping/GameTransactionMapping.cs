using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Mapping;

public static class GameTransactionMapping
{
    public static PaginationResult<ListTransactionExternalDto> ToSearchResult(this PaginationResult<Transaction> data)
    {
        var result = new PaginationResult<ListTransactionExternalDto>(
            items: [.. data.Items.Select(i => i.Adapt<ListTransactionExternalDto>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages, 
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}