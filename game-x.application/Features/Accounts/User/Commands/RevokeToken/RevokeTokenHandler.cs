using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;

namespace game_x.application.Features.Accounts.User.Commands.RevokeToken;

public sealed class RevokeTokenHandler(
    IUserAccessor userAccessor,
    IRefreshTokenManagerCacheService refreshTokenManager) : ICommandHandler<RevokeTokenCommand>
{
    public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        refreshTokenManager.RevokeToken(userId, request.TokenId);
        await Task.CompletedTask;
        return Unit.Value;
    }
}
