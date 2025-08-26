using game_x.application.Contract.Infrastructure.Caching;

namespace game_x.application.Features.Accounts.Admin.Queries.GetAllActiveTokensByAdmin;

public sealed class GetUserActiveTokensHandler(
    IRefreshTokenManagerCacheService refreshTokenManager) : IQueryHandler<GetUserActiveTokensByAdminQuery, GetUserActiveTokensDto[]>
{
    public Task<GetUserActiveTokensDto[]> Handle(GetUserActiveTokensByAdminQuery request, CancellationToken ct = default)
    {
        var result = refreshTokenManager.GetsByUserId(request.UserId)
            .Where(rt => !rt.IsRevoked || !rt.IsExpired)
            .Select(token => token.Adapt<GetUserActiveTokensDto>())
            .ToArray();
        return Task.FromResult(result);
    }
}
