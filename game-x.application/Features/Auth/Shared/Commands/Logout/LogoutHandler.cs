using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;

namespace game_x.application.Features.Auth.Shared.Commands.Logout;

public sealed class LogoutHandler(
    IUserAccessor userAccessor,
    IRefreshTokenManagerCacheService refreshTokenManager) : ICommandHandler<LogoutCommand>
{
    public Task<Unit> Handle(LogoutCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var jwtId = userAccessor.GetJwtId();
        var targetRefreshToken = refreshTokenManager.GetTokenByJwtId(userId, jwtId)
            ?? throw new UnauthorizedException();

        refreshTokenManager.RevokeToken(targetRefreshToken.UserId, targetRefreshToken.TokenHash);

        return Task.FromResult(Unit.Value);
    }
}
