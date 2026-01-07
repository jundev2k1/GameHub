using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Features.Games.Services;

namespace game_x.application.Features.UserWallet.Commands.RefreshWallet;

public sealed class RefreshWalletHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IGamePlatformService gamePlatformService,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<RefreshWalletCommand>
{
    public async Task<Unit> Handle(RefreshWalletCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        _ = await gamePlatformService.EnsureExternalAccountCreatedAsync(
            targetUser,
            request.GamePlatformId,
            ct: ct);
        var @event = new OnUserBalanceUpdatedEvent(userId, request.GamePlatformId);
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }
}
