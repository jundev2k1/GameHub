using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Client.Queries.GetMyTransactions;

public record GetMyTransactionsQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<ListTransactionInternalDto>>;