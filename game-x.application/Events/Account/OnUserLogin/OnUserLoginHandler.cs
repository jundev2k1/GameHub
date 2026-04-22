using game_x.application.Contract.Infrastructure.SignalR.Services;

namespace game_x.application.Events.Account.OnUserLogin;

public sealed class OnUserLoginHandler(IClientHubService clientHubService) : IApplicationEventHandler<OnUserLoginEvent>
{
    public async Task Handle(OnUserLoginEvent @event, CancellationToken ct = default)
    {
        await clientHubService.SendRevokeRefreshTokenAsync(@event.UserId);
    }
}