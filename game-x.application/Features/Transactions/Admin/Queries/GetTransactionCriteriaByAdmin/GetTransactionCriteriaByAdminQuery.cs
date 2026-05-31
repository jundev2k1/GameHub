using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Admin.Queries.GetTransactionCriteriaByAdmin;

public record GetTransactionCriteriaByAdminQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<ListTransactionInternalDto>>;