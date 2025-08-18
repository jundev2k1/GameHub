using game_x.application.Contract.Infrastructure.Caching;
using game_x.share.Helper;

namespace game_x.application.Features.Accounts.User.Commands.RevokeToken;

public sealed class RevokeTokenHandler(
    IRefreshTokenManagerCacheService refreshTokenManager) : ICommandHandler<RevokeTokenCommand>
{
    public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken ct = default)
    {
        refreshTokenManager.RevokeToken(HashHelper.Sha256(request.Token));
        await Task.CompletedTask;
        return Unit.Value;
    }
}
