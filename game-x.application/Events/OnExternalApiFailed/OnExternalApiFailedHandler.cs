using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Events.OnRefreshWalletFailed;

namespace game_x.application.Events.OnExternalApiFailed;

public sealed class OnExternalApiFailedHandler(IClientHubService clientHubService) : IApplicationEventHandler<OnExternalApiFailedEvent>
{
    public async Task Handle(OnExternalApiFailedEvent @event, CancellationToken ct = default)
    {
        switch (@event.Action)
        {
            case ExternalApiAction.SyncWallet:
                await clientHubService.NotifyWalletSynchronizationFailedAsync(@event.UserId, @event.PlatformId);
                break;

            default:
                break;
        }
    }
}
