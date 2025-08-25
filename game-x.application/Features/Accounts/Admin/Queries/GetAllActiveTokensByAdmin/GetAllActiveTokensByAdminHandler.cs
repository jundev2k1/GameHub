using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Caching;
using Mapster;

namespace game_x.application.Features.Accounts.Admin.Queries.GetAllActiveTokensByAdmin;

public sealed class GetAllActiveTokensHandler(
    IRefreshTokenManagerCacheService refreshTokenManager)
    : IQueryHandler<GetAllActiveTokensByAdminQuery, PaginationResult<GetAllActiveTokensDto>>
{
    public Task<PaginationResult<GetAllActiveTokensDto>> Handle(
        GetAllActiveTokensByAdminQuery request,
        CancellationToken ct = default)
    {
        var result = refreshTokenManager.GetRefreshTokenByCriteria(
            queryBuilder: query => query.Where(rt => !rt.IsRevoked && !rt.IsExpired),
            page: request.PageIndex ?? 1,
            pageSize: request.PageSize ?? 20,
            ct: ct
        );

        return Task.FromResult(new PaginationResult<GetAllActiveTokensDto>(
            result.Items.Select(token => token.Adapt<GetAllActiveTokensDto>()),
            result.TotalItems,
            result.TotalPages,
            result.PageNumber,
            result.PageSize
        ));
    }
}
