using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Infrastructure.SignalR.Services;

namespace game_x.application.Events.OnSendFriendRequest;

public sealed class OnSendFriendRequestHandler(IChatHubService chatHubService)
    : IApplicationEventHandler<OnSendFriendRequestEvent>
{
    public async Task Handle(OnSendFriendRequestEvent @event, CancellationToken ct = default)
    {
        var dto = @event.Dto.Adapt<FriendRequestSignalDto>();
        await chatHubService.SendFriendRequestAsync(dto);
    }
}