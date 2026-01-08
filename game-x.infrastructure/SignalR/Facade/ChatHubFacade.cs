using game_x.infrastructure.SignalR.Groups;

namespace game_x.infrastructure.SignalR.Facade;

using Microsoft.AspNetCore.SignalR;

public sealed class ChatHubFacade<THub, TClient>(IHubContext<THub, TClient> hub)
    where THub : Hub<TClient>
    where TClient : class
{
    public TClient BackOffice() => hub.Clients.Group(ChatGroups.BackOffice());
    public TClient Public() => hub.Clients.Group(ChatGroups.Public);
    public TClient PublicIdle() => hub.Clients.Group(ChatGroups.PublicIdle);
    public TClient OnlineAll() => hub.Clients.Group(ChatGroups.OnlineAll);
    public TClient IdleAgent() => hub.Clients.Group(ChatGroups.IdleAgent);
    public TClient AgentInbox() => hub.Clients.Group(ChatGroups.AgentInbox);
    public TClient IdleMember(string userId) => hub.Clients.Group(ChatGroups.IdleMember(userId));
    public TClient MemberInbox(string userId) => hub.Clients.Group(ChatGroups.MemberInbox(userId));
    public TClient Conversation(Guid convId) => hub.Clients.Group(ChatGroups.Conversation(convId));
}