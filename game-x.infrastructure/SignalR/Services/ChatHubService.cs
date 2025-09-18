using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.infrastructure.SignalR.Hubs;
using game_x.share.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Services;

public sealed class ChatHubService(IHubContext<ChatHub, IChatClient> hubContext)
    : IChatHubService, IHubServices
{
    public async Task SendSupportMessageAsync(CreatedMessageSignalResult res)
    {
        var msgDto = res.Msg;
        var conv = res.Conv;

        if (conv.CustomerId.IsNotNullOrEmpty())
        {
            await hubContext.Clients.Group(GroupNames.Member(conv.CustomerId!)).ConversationUpdated(conv);
            await hubContext.Clients.Group(GroupNames.Member(conv.CustomerId!)).MessageCreated(msgDto);
        }
        else if (conv.GuestId.IsNotNullOrEmpty())
        {
            await hubContext.Clients.Group(GroupNames.Guest(conv.GuestId!)).ConversationUpdated(conv);
            await hubContext.Clients.Group(GroupNames.Guest(conv.GuestId!)).MessageCreated(msgDto);
        }
        
        await hubContext.Clients.Group(GroupNames.Role(AppRoles.Admin)).ConversationUpdated(conv);
        await hubContext.Clients.Group(GroupNames.Role(AppRoles.Admin)).MessageCreated(msgDto);
        
        await hubContext.Clients.Group(GroupNames.Role(AppRoles.Cs)).ConversationUpdated(conv);
        await hubContext.Clients.Group(GroupNames.Role(AppRoles.Cs)).MessageCreated(msgDto);
    }
    
    public async Task SendFriendRequestAsync(FriendRequestSignalDto dto)
    {
        await hubContext.Clients.Group(GroupNames.Member(dto.AddresseeUserId!)).FriendRequest(dto);
    }
    
    public async Task SendFriendResponseAsync(FriendResponseSignalDto dto)
    {
        await hubContext.Clients.Group(GroupNames.Member(dto.RequesterUserId!)).FriendResponse(dto);
    }
    
    public async Task SendUnfriendAsync(UnfriendSignalDto dto)
    {
        await hubContext.Clients.Group(GroupNames.Member(dto.UnfriendedUserId!)).Unfriend(dto);
    }
    
    public async Task SendFriendBlockedAsync(FriendBlockedSignalDto dto)
    {
        await hubContext.Clients.Group(GroupNames.Member(dto.BlockedUserId!)).FriendBlocked(dto);
    }
    
    public async Task SendFriendUnblockedAsync(FriendBlockedSignalDto dto)
    {
        await hubContext.Clients.Group(GroupNames.Member(dto.BlockedUserId!)).FriendUnblocked(dto);
    }
}
