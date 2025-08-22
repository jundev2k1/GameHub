using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;

namespace game_x.application.Features.Accounts.Admin.Queries.GetAllActiveTokensByAdmin;

public sealed class GetAllActiveTokensHandler(
    IRefreshTokenManagerCacheService refreshTokenManager) : IQueryHandler<GetAllActiveTokensByAdminQuery, GetAllActiveTokensDto[]>
{
    public Task<GetAllActiveTokensDto[]> Handle(GetAllActiveTokensByAdminQuery request, CancellationToken ct = default)
    {
        var result = refreshTokenManager.GetsByUserId(request.UserId)
            .Where(rt => !rt.IsRevoked || !rt.IsExpired)
            .Select(token => token.Adapt<GetAllActiveTokensDto>())
            .ToArray();
        return Task.FromResult(result);
    }
}
