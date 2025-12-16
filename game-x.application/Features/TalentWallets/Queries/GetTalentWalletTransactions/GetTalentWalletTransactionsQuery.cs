using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.TalentWallets.DTOs;

namespace game_x.application.Features.TalentWallets.Queries.GetTalentWalletTransactions;

public record GetTalentWalletTransactionsQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<PaginationResult<TalentWalletTransactionDto>>;
