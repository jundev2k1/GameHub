using System.Text.Json;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Chat.Commands.SendMessage;
using game_x.application.Features.Chat.Commands.SendMessageToCustomer;
using game_x.application.Features.Chat.Commands.SendSupportMessage;
using game_x.application.Features.Chat.Commands.TouchDelivery;
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
    // Task InboxUpsert(InboxSignalDto signalDto); // sidebar badge/preview
    Task MemberAdded(ConversationMemberDto dto);
    /// <summary>Send it whenever a message is sent.</summary>
    Task MessageCreated(MessageSignalDto dto);
    Task MessageFailed(MessageFailedSignalDto signalDto);
    /// <summary>Notify that the user received a friend request</summary>
    Task FriendRequest(FriendRequestSignalDto dto);
    Task FriendResponse(FriendResponseSignalDto dto);
    Task Unfriend(UnfriendSignalDto dto);
    Task FriendBlocked(FriendBlockedSignalDto dto);
    Task FriendUnblocked(FriendBlockedSignalDto dto);
}

public sealed class ChatHub(
    ISender sender,
    IUserAccessor userAccessor,
    IConversationService convService,
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
    
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.OnlineAll);
            
            if (Context.User?.IsInRole(AppRoles.User) == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Member(userId));
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Role(AppRoles.User));
            }
        
            // If the user is an Admin/Cs, also add them to role groups (so they get broadcasts)
            if (Context.User?.IsInRole(AppRoles.Admin) == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Role(AppRoles.Admin));
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Admin(userId));
            }
    
            if (Context.User?.IsInRole(AppRoles.Cs) == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Role(AppRoles.Cs));
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Cs(userId));
            }
    
            logger.LogInformation("ChatHub {Role} connected: {UserId}",role.ToString(), userId);
        }
        else
        {
            userId = Context.UserIdentifier;
            if (string.IsNullOrWhiteSpace(userId)) { Context.Abort(); return; }
            role = AppRole.Of(AppRoles.Guest);
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Guest(userId));
        }
        
        logger.LogInformation("ChatHub {Role} connected: {UserId}", role.ToString(), userId);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        var user = Context.User;
        AppRole? role;
        string? userId;

        if (user?.Identity?.IsAuthenticated == true)
        {
            userId = userAccessor.GetUserId();
            role = userAccessor.GetRoles();
        }
        else
        {
            userId = Context.UserIdentifier;
            role = AppRole.Of(AppRoles.Guest);
        }
        logger.LogInformation("ChatHub {Role} disconnected: {UserId}",role.ToString(), userId);
        await base.OnDisconnectedAsync(ex);
    }

    public Task<string> Ping(object payload) => Task.FromResult($"pong:{JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true })}");
    
    // --- Subscription ---
    public Task JoinPublic() => Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Public);
    
    public async Task<Guid> OpenDm(string peerUserId)
    {
        var me = userAccessor.GetUserId();
        var ct = Context.ConnectionAborted;
        return await convService.EnsureForPair(me, peerUserId, ct);
    }
    
    public async Task<Guid> OpenSupport()
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
    
    public async Task JoinConversation(Guid convId)
    {
        await sender.Send(new TouchDeliveryCommand(convId));
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Conversation(convId));
    }
    
    public Task LeaveConversation(Guid convId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupNames.Conversation(convId));
    
    // --- Customer Support ---
    
    /// <summary>
    /// Send a support message by customer.
    /// Creates the user's support conversation if missing
    /// Broadcast to all Admin/Cs role groups
    /// </summary>
    [Authorize(Roles = AppRoles.User)]
    public async Task SendSupportMessage(SendSupportMessageCommand cmd)
    {
        try
        {
            var userId = userAccessor.GetUserId();
            var ct = Context.ConnectionAborted;
            await sender.Send(cmd with {SenderActorId = userId, SenderUserId = userId }, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending support message");
            await Clients.Caller.MessageFailed(new MessageFailedSignalDto(ClientLocalId: cmd.ClientLocalId));
        }
    }
    
    /// <summary>
    /// Send a support message by guest.
    /// Creates the guest's support conversation if missing
    /// Broadcast to all Admin/Cs role groups
    /// </summary>
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
    
    // --- Conversations & membership ---

    public async Task SendMessage(SendMessageCommand cmd)
    {
        try
        {
            var userId = userAccessor.GetUserId();
            var ct = Context.ConnectionAborted;
            await sender.Send(cmd with {SenderActorId = userId, SenderUserId = userId }, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error when sending message");
            await Clients.Caller.MessageFailed(new MessageFailedSignalDto(cmd.ClientLocalId, cmd.ConversationId));
        }
    }
}