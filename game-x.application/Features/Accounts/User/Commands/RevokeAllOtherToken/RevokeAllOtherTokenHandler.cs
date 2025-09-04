using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;

namespace game_x.application.Features.Accounts.User.Commands.RevokeAllOtherToken;

public sealed class RevokeAllOtherTokenHandler(
    IUserAccessor userAccessor,
    IRefreshTokenManagerCacheService refreshTokenManager) : ICommandHandler<RevokeAllOtherTokenCommand>
{
    public async Task<Unit> Handle(RevokeAllOtherTokenCommand request, CancellationToken ct = default)
    {
        var currentToken = userAccessor.GetJwtId();
        refreshTokenManager.GetsByUserId(userAccessor.GetUserId())
            .Where(t => t.JwtId != currentToken)
            .ToList()
            .ForEach(refreshTokenManager.RevokeToken);
        await Task.CompletedTask;
        return Unit.Value;
    }
}
