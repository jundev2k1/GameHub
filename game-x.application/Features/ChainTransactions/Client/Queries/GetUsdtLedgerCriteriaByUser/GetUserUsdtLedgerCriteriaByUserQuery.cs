using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.UserUsdtLedgers.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetUsdtLedgerCriteriaByUser;

public record GetUserUsdtLedgerCriteriaByUserQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<UserUsdtLedgerDto>>;