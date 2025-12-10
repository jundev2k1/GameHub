using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;

namespace game_x.application.Features.Games.WebHooks.Commands.OnWalletChanged;

public sealed class OnWalletChangedHandler(
    IUserRepo userRepo,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<OnWalletChangedCommand>
{
    public async Task<Unit> Handle(OnWalletChangedCommand request, CancellationToken ct = default)
    {
        var platformId = request.PartnerName switch
        {
            var name when name == PartnerName.Baccarat => GameConstants.PLATFORM_ID_GAMEBACCARAT,
            _ => throw new ForbiddenException()
        };
        var usrex = await userRepo.GetUserExtendByAccountAsync(platformId, request.Account, ct)
            ?? throw new NotFoundException(nameof(request.Account), request.Account);

        var @event = new OnUserBalanceUpdatedEvent(usrex.Id, platformId);
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }
}
