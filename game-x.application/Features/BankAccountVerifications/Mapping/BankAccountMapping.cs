using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.BankAccountVerifications.Dtos;

namespace game_x.application.Features.BankAccountVerifications.Mapping;

public static class BankAccountMapping
{
    public static PaginationResult<BankAccountListItemDto> ToSearchResult(this PaginationResult<UserBankAccount> data)
    {
        var result = new PaginationResult<BankAccountListItemDto>(
            items: [.. data.Items.Select(item => item.Adapt<BankAccountListItemDto>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}
