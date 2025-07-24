using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.BankAccountManagement.Dtos;

namespace game_x.application.Features.BankAccountManagement.Client.Queries.GetBankAccountCriteriaByUser;

public record GetBankAccountByCriteriaQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<BankAccountDto>>;
