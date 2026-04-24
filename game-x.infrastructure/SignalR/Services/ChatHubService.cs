using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Features.Chat.Dtos;
using game_x.infrastructure.SignalR.Facade;
using game_x.infrastructure.SignalR.Hubs;
using game_x.share.Extensions;

namespace game_x.infrastructure.SignalR.Services;

public sealed class ChatHubService(
    ActorHubFacade<ChatHub, IChatClient> actorHub,
    ChatHubFacade<ChatHub, IChatClient> chatHub)
    : IChatHubService, IHubServices
{
    public async Task SendSupportConversationUnreadAsync(IReadOnlyCollection<ConversationUnreadDto> dto)
    {
        await chatHub.BackOffice().SupportConversationUnread(dto);
    }
    
    public async Task SendSupportConversationClaimedAsync(ConversationSignalDto dto)
    {
        if (dto.Type == ConversationType.Support)
            await chatHub.BackOffice().ConversationClaimed(dto);
    }
    
    public async Task SendDeletedMessageAsync(DeletedMessageDto dto)
    {
        switch (dto.ConversationType)
        {
            case ConversationType.Public:
                await chatHub.Public().MessageDeleted(dto);
                break;
            
            default:
                await chatHub.Conversation(dto.ConversationId).MessageDeleted(dto);
                break;
        }
    }
    
    public async Task SendMarkAsReadAsync(ConversationSignalDto dto, string userId, AppRole role)
    {
        if(role.IsUser)
            await actorHub.Member(userId).ConversationUpdated(dto);
        
        if(role.IsBackOffice)
            await chatHub.BackOffice().ConversationUpdated(dto);
        
        if(role.IsGuest)
            await actorHub.Guest(userId).ConversationUpdated(dto);
    }
    
    public async Task SendTotalUnreadCountAsync(string userId, int totalUnreadCount)
    {
        await actorHub.Member(userId).TotalUnreadCount(totalUnreadCount);
    }
    
    public async Task SendPublicMessageAsync(CreatedMessageSignalResult res)
    {
        var msgDto = res.Msg;
        var inboxUpsert = res.InboxUpsert;
        await chatHub.Public().MessageCreated(msgDto);
        await chatHub.PublicIdle().InboxUpsert(inboxUpsert);
    }
    
    public async Task SendSupportMessageAsync(CreatedMessageSignalResult res)
    {
        var msgDto = res.Msg;
        var conv = res.Conv;

        if (conv.CustomerId.IsNotNullOrEmpty())
        {
            await actorHub.Member(conv.CustomerId!).ConversationUpdated(conv);
            await actorHub.Member(conv.CustomerId!).MessageCreated(msgDto);
        }
        else if (conv.GuestId.IsNotNullOrEmpty())
        {
            await actorHub.Guest(conv.GuestId!).ConversationUpdated(conv);
            await actorHub.Guest(conv.GuestId!).MessageCreated(msgDto);
        }
        await chatHub.BackOffice().ConversationUpdated(conv);
        await chatHub.BackOffice().MessageCreated(msgDto);
    }
    
    public async Task SendSupportMessageV2Async(CreatedMessageSignalResult res)
    {
        var msgDto = res.Msg;
        var conv = res.Conv;
        var inboxUpsert = res.InboxUpsert;

        await chatHub.Conversation(conv.ConversationId).MessageCreated(msgDto);
        
        await chatHub.AgentInbox().ConversationUpdated(conv);
        await chatHub.IdleAgent().InboxUpsert(inboxUpsert);
        
        if (conv.CustomerId.IsNotNullOrEmpty())
        {
            await chatHub.MemberInbox(conv.CustomerId!).ConversationUpdated(conv);
            await chatHub.IdleMember(conv.CustomerId!).InboxUpsert(inboxUpsert);
        }
        else if (conv.GuestId.IsNotNullOrEmpty())
        {
            await chatHub.MemberInbox(conv.GuestId!).ConversationUpdated(conv);
            await chatHub.IdleMember(conv.GuestId!).InboxUpsert(inboxUpsert);
        }
    }
    
    public async Task SendDirectMessageAsync(CreatedMessageSignalResult res, ConvMemberDto[] members)
    {
        var msgDto = res.Msg;
        var conv = res.Conv;
        var upsertedInbox = res.InboxUpsert;
        
        
        await chatHub.Conversation(conv.ConversationId).MessageCreated(msgDto);
        
        foreach (var member in members)
        {
            await chatHub.MemberInbox(member.UserId).ConversationUpdated(conv with {ClientUnreadCount = member.UnreadCount});
            if(member.IsHidden != true)
                await chatHub.IdleMember(member.UserId).InboxUpsert(upsertedInbox);
        }
    }
    
    public async Task SendFriendRequestAsync(FriendRequestSignalDto dto)
    {
        await actorHub.Member(dto.AddresseeUserId!).FriendRequest(dto);
    }
    
    public async Task SendFriendResponseAsync(FriendResponseSignalDto dto, ConversationSignalDto? conv = null)
    {
        await actorHub.Member(dto.RequesterUserId!).FriendResponse(dto);
        if (dto.State == SocialLinkState.Accepted && conv != null)
        {
            await actorHub.Member(dto.RequesterUserId!).ConversationUpdated(conv with
            {
                CustomerAvatarUrl = dto.AddresseeAvatarUrl,
                CustomerDisplayName = dto.AddresseeNickname
            });
            await actorHub.Member(dto.AddresseeUserId!).ConversationUpdated(conv with
            {
                CustomerAvatarUrl = dto.RequesterAvatarUrl,
                CustomerDisplayName = dto.RequesterNickname
            });
        }
    }
    
    public async Task SendUnfriendAsync(UnfriendSignalDto dto)
    {
        await actorHub.Member(dto.UnfriendedUserId!).Unfriend(dto);
    }
    
    public async Task SendFriendBlockedAsync(FriendBlockedSignalDto dto)
    {
        await actorHub.Member(dto.BlockedUserId!).FriendBlocked(dto);
    }
    
    public async Task SendFriendUnblockedAsync(FriendBlockedSignalDto dto)
    {
        await actorHub.Member(dto.BlockedUserId!).FriendUnblocked(dto);
    }
}