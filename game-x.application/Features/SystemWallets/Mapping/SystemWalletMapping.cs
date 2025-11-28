using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.SystemWallets.DTOs;

namespace game_x.application.Features.SystemWallets.Mapping;

public static class SystemWalletMapping
{
    public static PaginationResult<SystemWalletTransactionDto> ToSearchResult(this PaginationResult<SystemWalletTransaction> input)
    {
        return new PaginationResult<SystemWalletTransactionDto>(
            input.Items.Select(i => i.Adapt<SystemWalletTransactionDto>()),
            input.TotalItems,
            input.TotalPages,
            input.PageNumber,
            input.PageSize);
    }
}
