using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.BankAccountVerifications.Dtos;

namespace game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountByCriteria;

public record GetBankAccountByCriteriaQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<BankAccountListItemDto>>;
