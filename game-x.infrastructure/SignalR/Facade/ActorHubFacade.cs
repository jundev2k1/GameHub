using game_x.infrastructure.SignalR.Groups;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Facade;

public sealed class ActorHubFacade<THub, TClient>(IHubContext<THub, TClient> hub)
    where THub : Hub<TClient>
    where TClient : class
{
    public TClient All() => hub.Clients.All;
    public TClient AdminAll() => hub.Clients.Group(ActorGroups.Broadcast(AppRoles.Admin));
    public TClient CsAll() => hub.Clients.Group(ActorGroups.Broadcast(AppRoles.Cs));
    public TClient MemberAll() => hub.Clients.Group(ActorGroups.Broadcast(AppRoles.User));
    public TClient Admin(string userId) => hub.Clients.Group(ActorGroups.Admin(userId));
    public TClient Cs(string userId) => hub.Clients.Group(ActorGroups.Cs(userId));
    public TClient Member(string userId) => hub.Clients.Group(ActorGroups.Member(userId));
    public TClient Guest(string userId) => hub.Clients.Group(ActorGroups.Guest(userId));
}