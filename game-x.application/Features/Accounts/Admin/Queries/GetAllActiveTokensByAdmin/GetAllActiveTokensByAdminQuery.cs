using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;

namespace game_x.application.Features.Accounts.Admin.Queries.GetAllActiveTokensByAdmin;

public sealed record GetAllActiveTokensByAdminQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize
) : IQuery<PaginationResult<GetAllActiveTokensDto>>;

public sealed record GetAllActiveTokensDto(
    Guid PublicId,
    string IpAddress,
    string UserAgent,
    string DeviceInfo,
    DateTime CreatedAt);
