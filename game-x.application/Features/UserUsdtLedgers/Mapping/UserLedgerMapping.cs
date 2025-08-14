using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.UserUsdtLedgers.Dtos;

namespace game_x.application.Features.UserUsdtLedgers.Mapping;

public static class UserLedgerMapping
{
    public static PaginationResult<UserUsdtLedgerDto> ToSearchResult(this PaginationResult<UserUsdtLedger> data)
    {
        var result = new PaginationResult<UserUsdtLedgerDto>(
            items: [.. data.Items.Select(item => item.Adapt<UserUsdtLedgerDto>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}