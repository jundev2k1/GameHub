using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.SystemWallets.DTOs;

namespace game_x.application.Features.SystemWallets.Queries.GetSystemWalletTransactions;

public record GetSystemWalletTransactionsQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<PaginationResult<SystemWalletTransactionDto>>;
