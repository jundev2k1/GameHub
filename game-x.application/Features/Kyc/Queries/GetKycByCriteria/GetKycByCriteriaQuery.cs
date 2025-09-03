using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.Kyc.Dtos;

namespace game_x.application.Features.Kyc.Queries.GetKycByCriteria;

public record GetKycByCriteriaQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<UserKycListItemDto>>;
