using game_x.application.Contract.Infrastructure.Caching;

namespace game_x.application.Features.Accounts.User.Commands.RevokeToken;

public sealed class RevokeTokenHandler(
    IRefreshTokenManagerCacheService refreshTokenManager) : ICommandHandler<RevokeTokenCommand>
{
    public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken ct = default)
    {
        refreshTokenManager.RevokeToken(request.UserId, request.TokenId);
        await Task.CompletedTask;
        return Unit.Value;
    }
}
