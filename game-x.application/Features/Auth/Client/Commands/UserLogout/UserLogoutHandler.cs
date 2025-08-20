using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;

namespace game_x.application.Features.Auth.Client.Commands.UserLogout;

public sealed class UserLogoutHandler(
    IUserAccessor userAccessor,
    IRefreshTokenManagerCacheService refreshTokenManager) : ICommandHandler<UserLogoutCommand>
{
    public async Task<Unit> Handle(UserLogoutCommand request, CancellationToken ct = default)
    {
        var accessTokenInfo = userAccessor.GetTokenInfo();
        var targetRefreshToken = refreshTokenManager.GetTokenByJwtId(accessTokenInfo.Subject!, accessTokenInfo.JwtId!)
            ?? throw new UnauthorizedException();

        refreshTokenManager.RevokeToken(targetRefreshToken.UserId, targetRefreshToken.TokenHash);

        return await Task.FromResult(Unit.Value);
    }
}
