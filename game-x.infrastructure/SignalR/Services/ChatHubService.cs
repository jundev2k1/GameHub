using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Features.Chat.Dtos;
using game_x.infrastructure.SignalR.Hubs;
using game_x.share.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Services;

public sealed class ChatHubService(IHubContext<ChatHub, IChatClient> hubContext)
    : IChatHubService, IHubServices
{
    public async Task SendDeletedMessageAsync(DeletedMessageDto dto)
    {
        switch (dto.ConversationType)
        {
            case ConversationType.Public:
                await hubContext.Clients.Group(GroupNames.Public).MessageDeleted(dto);
                break;
            
            default:
                await hubContext.Clients.Group(GroupNames.Conversation(dto.ConversationId)).MessageDeleted(dto);
                break;
        }
    }
    
    public async Task SendMarkAsReadAsync(ConvUnreadDto res, string userId)
    {
        await hubContext.Clients.Group(GroupNames.Member(userId)).MarkAsRead(res);
    }
    
    public async Task SendPublicMessageAsync(CreatedMessageSignalResult res)
    {
        var msgDto = res.Msg;
        var inboxUpsert = res.InboxUpsert;
        await hubContext.Clients.Group(GroupNames.Public).MessageCreated(msgDto);
        await hubContext.Clients.Group(GroupNames.PublicIdle).InboxUpsert(inboxUpsert);
    }
    
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
    
    public async Task SendSupportMessageV2Async(CreatedMessageSignalResult res)
    {
        var msgDto = res.Msg;
        var conv = res.Conv;
        var inboxUpsert = res.InboxUpsert;

        await hubContext.Clients.Group(GroupNames.Conversation(conv.ConversationId)).MessageCreated(msgDto);
        await hubContext.Clients.Group(GroupNames.AgentInbox).ConversationUpdated(conv);
        await hubContext.Clients.Group(GroupNames.IdleAgent).InboxUpsert(inboxUpsert);
        
        if (conv.CustomerId.IsNotNullOrEmpty())
        {
            await hubContext.Clients.Group(GroupNames.MemberInbox(conv.CustomerId!)).ConversationUpdated(conv);
            await hubContext.Clients.Group(GroupNames.IdleMember(conv.CustomerId!)).InboxUpsert(inboxUpsert);
        }
        else if (conv.GuestId.IsNotNullOrEmpty())
        {
            await hubContext.Clients.Group(GroupNames.MemberInbox(conv.GuestId!)).ConversationUpdated(conv);
            await hubContext.Clients.Group(GroupNames.IdleMember(conv.GuestId!)).InboxUpsert(inboxUpsert);
        }
    }
    
    public async Task SendDirectMessageAsync(CreatedMessageSignalResult res, ConvMemberDto[] members)
    {
        var msgDto = res.Msg;
        var conv = res.Conv;
        var upsertedInbox = res.InboxUpsert;
        
        await hubContext.Clients.Group(GroupNames.Conversation(conv.ConversationId)).MessageCreated(msgDto);
        
        foreach (var member in members)
        {
            await hubContext.Clients.Group(GroupNames.MemberInbox(member.UserId)).ConversationUpdated(conv);
            if(member.IsHidden != true)
                await hubContext.Clients.Group(GroupNames.IdleMember(member.UserId)).InboxUpsert(upsertedInbox);
        }
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
