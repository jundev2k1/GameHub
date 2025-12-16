using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.TalentWallets.DTOs;

namespace game_x.application.Features.TalentWallets.Mapping;

public static class TalentWalletTransactionMapping
{
    public static PaginationResult<TalentWalletTransactionDto> ToSearchResult(this PaginationResult<TalentWalletTransaction> input)
    {
        return new PaginationResult<TalentWalletTransactionDto>(
            [.. input.Items.Select(i => i.Adapt<TalentWalletTransactionDto>())],
            input.TotalItems,
            input.TotalPages,
            input.PageNumber,
            input.PageSize);
    }
}
