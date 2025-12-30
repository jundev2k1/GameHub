using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Client.Queries.GetMyWalletTransactions;

public record GetMyWalletTransactionsQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize,
    string Mode) : IQuery<PaginationResult<WalletTransactionDto>>;