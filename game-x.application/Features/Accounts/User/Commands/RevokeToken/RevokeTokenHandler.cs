using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.share.Helper;

namespace game_x.application.Features.Accounts.User.Commands.RevokeToken;

public sealed class RevokeTokenHandler(
    IUserAccessor userAccessor,
    IRefreshTokenManagerCacheService refreshTokenManager) : ICommandHandler<RevokeTokenCommand>
{
    public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        refreshTokenManager.RevokeToken(userId, HashHelper.Sha256(request.Token));
        await Task.CompletedTask;
        return Unit.Value;
    }
}
