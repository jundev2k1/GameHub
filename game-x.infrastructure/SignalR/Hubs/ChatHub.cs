using System.Text.Json;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Chat.Commands.MarkMessageAsRead;
using game_x.application.Features.Chat.Commands.SendMessage;
using game_x.application.Features.Chat.Commands.SendMessageToCustomer;
using game_x.application.Features.Chat.Commands.SendSupportMessage;
using game_x.application.Features.Chat.Commands.TouchDelivery;
using game_x.application.Features.Chat.Dtos;
using game_x.infrastructure.SignalR.Groups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace game_x.infrastructure.SignalR.Hubs;

public interface IChatClient
{
    /// <summary>
    ///     Send it whenever there is any update in the conversation, such as sending a message, reacting, etc.
    /// </summary>
    Task ConversationUpdated(ConversationSignalDto signalDto); // sidebar badge/preview
    Task InboxUpsert(InboxUpsertSignalDto signalDto); // sidebar badge/preview
    Task MemberAdded(ConversationMemberDto dto);
    /// <summary>Send it whenever a message is sent.</summary>
    Task MessageCreated(MessageSignalDto dto);
    Task MessageDeleted(DeletedMessageDto dto);
    Task MarkAsRead(ConvUnreadDto dto);
    Task TotalUnreadCount(int totalUnreadCount);
    Task MessageFailed(MessageFailedSignalDto signalDto);
    /// <summary>Notify that the user received a friend request</summary>
    Task FriendRequest(FriendRequestSignalDto dto);
    Task FriendResponse(FriendResponseSignalDto dto);
    Task Unfriend(UnfriendSignalDto dto);
    Task FriendBlocked(FriendBlockedSignalDto dto);
    Task FriendUnblocked(FriendBlockedSignalDto dto);
    Task SupportConversationUnread(IReadOnlyCollection<ConversationUnreadDto> dto);
    Task ConversationClaimed(ConversationSignalDto signalDto);
}

public sealed class ChatHub(
    ISender sender,
    IUserAccessor userAccessor,
    IConversationService convService,
    IConversationRepo conversationRepo,
    IConversationMemberRepo convMemberRepo,
    ILogger<ChatHub> logger) : Hub<IChatClient>
{
    public const string Path = "/hubs/chat";
    
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;
        AppRole? role;
        string? userId;
        
        if (user?.Identity?.IsAuthenticated == true)
        {
            userId = userAccessor.GetUserId();
            role = userAccessor.GetRoles();
    
            await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.OnlineAll);
            
            if (Context.User?.IsInRole(AppRoles.User) == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, ActorGroups.Member(userId));
                await Groups.AddToGroupAsync(Context.ConnectionId, ActorGroups.Broadcast(AppRoles.User));
            }
        
            // If the user is an Admin/Cs, also add them to role groups (so they get broadcasts)
            if (Context.User?.IsInRole(AppRoles.Admin) == true || Context.User?.IsInRole(AppRoles.Cs) == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.BackOffice());
            }
    
            logger.LogInformation("ChatHub {Role} connected: {UserId}",role.ToString(), userId);
        }
        else
        {
            userId = Context.UserIdentifier;
            if (string.IsNullOrWhiteSpace(userId)) { Context.Abort(); return; }
            role = AppRole.Of(AppRoles.Guest);
            await Groups.AddToGroupAsync(Context.ConnectionId, ActorGroups.Guest(userId));
        }
        
        logger.LogInformation("ChatHub {Role} connected: {UserId}", role.ToString(), userId);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        var user = Context.User;
        AppRole? role;
        string userId;

        if (user?.Identity?.IsAuthenticated == true)
        {
            userId = userAccessor.GetUserId();
            role = userAccessor.GetRoles();
        }
        else
        {
            userId = Context.UserIdentifier ?? throw new NullReferenceException();
            role = AppRole.Of(AppRoles.Guest);
        }
        logger.LogInformation("ChatHub {Role} disconnected: {UserId}",role.ToString(), userId);
        await base.OnDisconnectedAsync(ex);
    }

    public Task<string> Ping(object payload) => Task.FromResult($"pong:{JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true })}");
    
    // --- Subscription ---
    [AllowAnonymous]
    public async Task JoinIdle()
    {
        try
        {
            var user = Context.User;
            AppRole? role;
            string userId;

            if (user?.Identity?.IsAuthenticated == true)
            {
                userId = userAccessor.GetUserId();
                role = userAccessor.GetRoles();
            }
            else
            {
                userId = Context.UserIdentifier ?? throw new NullReferenceException();
                role = AppRole.Of(AppRoles.Guest);
            }
            
            if(role.IsBackOffice)
                await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.IdleAgent);
            else
                await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.IdleMember(userId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error leaving idle");
            throw;
        }
    }
    
    [Authorize(Roles = AppRoles.User)]
    public async Task JoinPublicIdle()
    {
        try
        {
            var ct = Context.ConnectionAborted;
            var userId = userAccessor.GetUserId();
            var existedConv = await conversationRepo.FindPublicAsync(ct)
                ?? throw new NotFoundException(MessageCode.Chatting.ConversationNotFound);
            var member = await convMemberRepo.GetByConvIdAndUserIdAsync(existedConv.PublicId, userId, ct);
            
            if(member?.IsHidden != true)
                await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.PublicIdle, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error joining public idle");
            throw;
        }
    }
    
    [AllowAnonymous]
    public async Task LeaveIdle()
    {
        try
        {
            var user = Context.User;
            AppRole? role;
            string userId;

            if (user?.Identity?.IsAuthenticated == true)
            {
                userId = userAccessor.GetUserId();
                role = userAccessor.GetRoles();
            }
            else
            {
                userId = Context.UserIdentifier ?? throw new NullReferenceException();
                role = AppRole.Of(AppRoles.Guest);
            }
            
            if(role.IsBackOffice)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroups.IdleAgent);
            else
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroups.IdleMember(userId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error leaving idle");
            throw;
        }
    }
    
    [Authorize(Roles = AppRoles.User)]
    public async Task LeavePublicIdle()
    {
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroups.PublicIdle);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error leaving public idle");
            throw;
        }
    }
    
    [Authorize(Roles = AppRoles.User)]
    public async Task<Guid> OpenPublic()
    {
        try
        {
            var ct = Context.ConnectionAborted;
            return await convService.EnsureForPublic(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error opening public");
            throw;
        }
    }

    [Authorize(Roles = AppRoles.User)]
    public async Task JoinPublic()
    {
        try
        {
            var ct = Context.ConnectionAborted;
            var existedConv = await conversationRepo.FindPublicAsync(ct)
                ?? throw new NotFoundException(MessageCode.Chatting.ConversationNotFound);
            
            await sender.Send(new TouchDeliveryCommand(existedConv.PublicId), ct);
            await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.Public, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error joining public");
            throw;
        }
    }
    
    [Authorize(Roles = AppRoles.User)]
    public Task LeavePublic()
    {
        try
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroups.Public);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error leaving public");
            throw;
        }
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs},{AppRoles.User}")]
    public Task JoinInbox()
    {
        try
        {
            var role = userAccessor.GetRoles();
            if (role.IsUser)
            {
                var me = userAccessor.GetUserId();
                return Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.MemberInbox(me));
            }
            return Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.AgentInbox);
            
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error joining inbox");
            throw;
        }
    }
    
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs},{AppRoles.User}")]
    public Task LeaveInbox()
    {
        try
        {
            var role = userAccessor.GetRoles();
            if (role.IsUser)
            {
                var me = userAccessor.GetUserId();
                return Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroups.MemberInbox(me));
            }
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroups.AgentInbox);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error leaving inbox");
            throw;
        }
    }
    
    [Authorize(Roles = AppRoles.User)]
    public async Task<Guid> OpenConversation(string peerUserId)
    {
        var me = userAccessor.GetUserId();
        try
        {
            var ct = Context.ConnectionAborted;
            var conv = await convService.EnsureForPair(me, peerUserId, ct);
            return conv.PublicId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error opening conversation between ${me} and ${peerUserId}");
            throw;
        }
    }
    
    [Authorize(Roles = AppRoles.User)]
    public async Task JoinConversation(Guid convId)
    {
        try
        {
            var user = Context.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                await sender.Send(new TouchDeliveryCommand(convId));
            }
            
            await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.Conversation(convId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error joining conversation ${convId}");
            throw;
        }
    }

    [Authorize(Roles = AppRoles.User)]
    public Task LeaveConversation(Guid convId)
    { 
        try
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroups.Conversation(convId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error leaving conversation ${convId}");
            throw;
        }
    }
    
    [AllowAnonymous]
    public async Task<Guid> OpenSupport()
    {
        try
        {
            var user = Context.User;
            string? actorId;
            string? userId = null;

            if (user?.Identity?.IsAuthenticated == true)
            {
                actorId = userAccessor.GetUserId();
                userId = userAccessor.GetUserId();
            }
            else
            {
                actorId = Context.UserIdentifier;
            }

            var ct = Context.ConnectionAborted;
            return await convService.EnsureForSupport(actorId ?? string.Empty, userId, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error opening support conversation");
            throw;
        }
    }
    
    /// <summary>Join the support conversation</summary>
    [AllowAnonymous]
    public async Task JoinSupport(Guid convId)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ChatGroups.Conversation(convId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error joining conversation ${convId}");
            throw;
        }
    }
    
    [AllowAnonymous]
    public Task LeaveSupport(Guid convId)
    { 
        try
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatGroups.Conversation(convId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error leaving conversation ${convId}");
            throw;
        }
    }

    [Authorize(Roles = AppRoles.User)]
    public async Task SendSupportMessage(SendSupportMessageCommand cmd)
    {
        try
        {
            var userId = userAccessor.GetUserId();
            var ct = Context.ConnectionAborted;
            await sender.Send(cmd with {SenderActorId = userId, SenderUserId = userId}, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending support message");
            await Clients.Caller.MessageFailed(new MessageFailedSignalDto(ClientLocalId: cmd.ClientLocalId));
        }
    }
    
    public async Task SendSupportMessageByGuest(SendSupportMessageCommand cmd)
    {
        try
        {
            var guestId = Context.UserIdentifier;
            if (string.IsNullOrWhiteSpace(guestId)) { return; }
            var ct = Context.ConnectionAborted;
            await sender.Send(cmd with {SenderActorId = guestId}, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending support message");
            await Clients.Caller.MessageFailed(new MessageFailedSignalDto(ClientLocalId: cmd.ClientLocalId));
        }
    }
    
    /// <summary>Send a support message to customer.</summary>
    [Obsolete("Use SendMessageByAgent instead")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    public async Task SendSupportMessageToCustomer(SendMessageToCustomerCommand cmd)
    {
        try
        {
            var ct = Context.ConnectionAborted;
            await sender.Send(cmd, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending support message");
            await Clients.Caller.MessageFailed(new MessageFailedSignalDto(
                ClientLocalId: cmd.ClientLocalId,
                ConversationId: cmd.ConversationId));
        }
    }

    [Authorize(Roles = AppRoles.User)]
    public async Task SendMessageByMember(SendMessageCommand cmd)
    {
        try
        {
            var userId = userAccessor.GetUserId();
            var ct = Context.ConnectionAborted;
            await sender.Send(cmd with
            {
                SenderActorId = userId, 
                SenderUserId = userId,
                Kind = MessageKind.Text
            }, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error when sending message");
            await Clients.Caller.MessageFailed(new MessageFailedSignalDto(cmd.ClientLocalId, cmd.ConversationId));
        }
    }
    
    /// <summary>
    /// Send a support message by guest.
    /// Creates the guest's support conversation if missing
    /// Broadcast to all Admin/Cs role groups
    /// </summary>
    public async Task SendMessageByGuest(SendMessageCommand cmd)
    {
        try
        {
            var guestId = Context.UserIdentifier;
            if (string.IsNullOrWhiteSpace(guestId)) { return; }
            var ct = Context.ConnectionAborted;
            await sender.Send(cmd with
            {
                SenderActorId = guestId,
                Kind = MessageKind.Text
            }, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending support message");
            await Clients.Caller.MessageFailed(new MessageFailedSignalDto(ClientLocalId: cmd.ClientLocalId));
        }
    }
    
    /// <summary>Send a support message to customer.</summary>
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    public async Task SendMessageByAgent(SendMessageCommand cmd)
    {
        try
        {
            var userId = userAccessor.GetUserId();
            var ct = Context.ConnectionAborted;
            await sender.Send(cmd with
            {
                IsAgent = true, 
                SenderActorId = userId, 
                SenderUserId = userId,
                Kind = MessageKind.Text
            }, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending support message");
            await Clients.Caller.MessageFailed(new MessageFailedSignalDto( cmd.ClientLocalId, cmd.ConversationId));
        }
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs},{AppRoles.User}")]
    public async Task MarkAsRead(MarkMessageAsReadCommand cmd)
    {
        try
        {
            await sender.Send(cmd);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error reading message");
            throw;
        }
    }
    
    [AllowAnonymous]
    public async Task MarkAsReadForGuest(MarkMessageAsReadCommand cmd)
    {
        try
        {
            var guestId = Context.UserIdentifier;
            if (string.IsNullOrWhiteSpace(guestId)) { return; }
            var ct = Context.ConnectionAborted;
            await sender.Send(cmd with {GuestId = guestId}, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error reading message");
            throw;
        }
    }
}