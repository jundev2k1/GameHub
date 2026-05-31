using game_x.infrastructure.SignalR.Groups;

namespace game_x.infrastructure.SignalR.Facade;

using Microsoft.AspNetCore.SignalR;

public sealed class LiveStreamHubFacade<THub, TClient>(IHubContext<THub, TClient> hub)
    where THub : Hub<TClient>
    where TClient : class
{
    public TClient Stream(string streamKey) => hub.Clients.Group(LiveStreamGroups.Stream(streamKey));
    public TClient StreamMember(string streamKey, string userId) => hub.Clients.Group(LiveStreamGroups.StreamMember(streamKey, userId));
    public TClient StreamHost(string streamKey) => hub.Clients.Group(LiveStreamGroups.StreamHost(streamKey));
}