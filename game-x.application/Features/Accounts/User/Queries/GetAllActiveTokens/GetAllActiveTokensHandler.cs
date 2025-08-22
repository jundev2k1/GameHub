using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;

namespace game_x.application.Features.Accounts.User.Queries.GetAllActiveTokens;

public sealed class GetAllActiveTokensHandler(
    IUserAccessor userAccessor,
    IRefreshTokenManagerCacheService refreshTokenManager) : IQueryHandler<GetAllActiveTokensQuery, GetAllActiveTokensDto[]>
{
    public Task<GetAllActiveTokensDto[]> Handle(GetAllActiveTokensQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var currentJwtId = userAccessor.GetJwtId();
        var result = refreshTokenManager.GetsByUserId(userId)
            .Where(rt => !rt.IsRevoked && !rt.IsExpired)
            .Select(token => token.Adapt<GetAllActiveTokensDto>() with
            {
                IsCurrentToken = token.JwtId == currentJwtId
            })
            .ToArray();
        return Task.FromResult(result);
    }
}
