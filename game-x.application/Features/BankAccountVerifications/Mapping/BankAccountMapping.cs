using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountByCriteria;

namespace game_x.application.Features.BankAccountVerifications.Mapping;

public static class BankAccountMapping
{
    public static PaginationResult<GetBankAccountByCriteriaSearchItem> ToSearchResult(this PaginationResult<UserBankAccount> data)
    {
        var result = new PaginationResult<GetBankAccountByCriteriaSearchItem>(
            items: [.. data.Items.Select(item => item.Adapt<GetBankAccountByCriteriaSearchItem>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize);
        return result;
    }
}
