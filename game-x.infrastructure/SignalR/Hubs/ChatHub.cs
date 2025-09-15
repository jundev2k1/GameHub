using System.Text.Json;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Features.Chat.Commands.SendMessageToCustomer;
using game_x.application.Features.Chat.Commands.SendSupportMessage;
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
    Task ConversationUpdated(ConversationSignalDto signalDto);
    Task MemberAdded(ConversationMemberDto dto);
    /// <summary>Send it whenever a message is sent.</summary>
    Task MessageCreated(MessageSignalDto dto);
    Task MessageFailed(MessageFailedSignalDto signalDto);
    /// <summary>Notify that the user received a friend request</summary>
    Task FriendRequest(FriendRequestSignalDto dto);
    // Task MemberRemoved(ConversationMemberDto dto);
    //
    // Task MessageCreated(MessageDto dto);
    // Task MessageEdited(MessageDto dto);
    // Task MessageDeleted(MessageDeletedDto dto);
    //
    // Task ReadUpdated(ReadPointerDto dto); // last-read pointer per user
    //
    // // ----- V2 -----
    // Task SendRejected(SendRejectedDto dto); // reason = BlockedByRecipient, NotMember, etc.
    //
    // // ----- V3 -----
    // Task ReactionUpdated(ReactionChangedDto dto);   // emoji + who + counts
    //
    // // ----- V4 -----
    // Task AttachmentPlaceholderCreated(AttachmentPlaceholderDto dto); // Pending
    // Task AttachmentLinked(AttachmentLinkedDto dto);                  // Linked + MediaFileId
    // Task AttachmentFailed(AttachmentFailedDto dto);                  // Failed + ErrorCode
    // Task AttachmentProgress(AttachmentProgressDto dto);              // optional: percent, bytes
    //
    // // ----- V5 -----
    // Task Typing(TypingDto dto); // userId, conversationId, isTyping
    //
    // // ----- V6 -----
    // Task ConversationUpdated(ConversationDto dto); // status or assigned agent changed
    // Task DeliveryUpdated(DeliveryDto dto);         // per-member LastDeliveredAt if you track it
}

public sealed class ChatHub(
    ISender sender,
    IUserAccessor userAccessor,
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

    public Task<string> Ping(dynamic payload) => Task.FromResult($"pong:{JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true })}");
    
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

    // Create DM or Support conversation; returns dto and adds creator to group
    // public async Task<ConversationDto> CreateConversation(CreateConversationCommand cmd, CancellationToken ct = default)
    // {
    //     cmd.UserId ??= userAccessor.GetUserId();
    //     var dto = await sender.Send(cmd, ct);
    //     await Groups.AddToGroupAsync(Context.ConnectionId, CG(dto.Id));
    //     await Clients.Group(UG(cmd.UserId!)).ConversationCreated(dto);
    //     return dto;
    // }
    //
    // public async Task JoinConversation(int conversationId, CancellationToken ct = default)
    // {
    //     // authorize membership
    //     await sender.Send(new EnsureMemberCommand(conversationId, userAccessor.GetUserId()), ct);
    //     await Groups.AddToGroupAsync(Context.ConnectionId, CG(conversationId));
    // }
    //
    // public async Task LeaveConversation(int conversationId, CancellationToken ct = default)
    // {
    //     await Groups.RemoveFromGroupAsync(Context.ConnectionId, CG(conversationId));
    // }

    // --- Messages ---

    // public async Task<MessageDto> SendTextMessage(SendTextMessageCommand cmd, CancellationToken ct = default)
    // {
    //     cmd.SenderUserId ??= userAccessor.GetUserId();
    //     var dto = await sender.Send(cmd, ct); // validates: member, not blocked, etc.
    //     await Clients.Group(CG(dto.ConversationId)).MessageCreated(dto);
    //     return dto;
    // }

    // public async Task<MessageDto> EditMessage(EditMessageCommand cmd, CancellationToken ct = default)
    // {
    //     cmd.EditorUserId ??= userAccessor.GetUserId();
    //     var dto = await sender.Send(cmd, ct);
    //     await Clients.Group(CG(dto.ConversationId)).MessageEdited(dto);
    //     return dto;
    // }
    //
    // public async Task MessageDelete(DeleteMessageCommand cmd, CancellationToken ct = default)
    // {
    //     cmd.RequesterUserId ??= userAccessor.GetUserId();
    //     var dto = await sender.Send(cmd, ct); // returns ConversationId + MessageId
    //     await Clients.Group(CG(dto.ConversationId)).MessageDeleted(new MessageDeletedDto(dto.ConversationId, dto.MessageId));
    // }

    // --- Read pointer ---

    // public async Task SetLastRead(SetLastReadCommand cmd, CancellationToken ct = default)
    // {
    //     cmd.UserId ??= userAccessor.GetUserId();
    //     var dto = await sender.Send(cmd, ct); // includes ConversationId, UserId, LastReadMessageId
    //     await Clients.Group(CG(dto.ConversationId)).ReadUpdated(dto);
    // }
    
    // ----- V3 -----
    
    // public async Task ToggleReaction(ToggleReactionCommand cmd, CancellationToken ct = default)
    // {
    //     cmd.UserId ??= userAccessor.GetUserId();
    //     var dto = await sender.Send(cmd, ct); // returns ConversationId, MessageId, Emoji, Counts, UserIds?
    //     await Clients.Group(CG(dto.ConversationId)).ReactionUpdated(dto);
    // }
    
    // ----- V4 -----
    
    // public async Task<AttachmentPlaceholderDto> CreateAttachmentPlaceholder(CreateAttachmentPlaceholderCommand cmd, CancellationToken ct = default)
    // {
    //     cmd.RequesterUserId ??= userAccessor.GetUserId();
    //     var dto = await sender.Send(cmd, ct); // creates MessageAttachment with MediaFileId=null, BindingStatus=Pending
    //     await Clients.Group(CG(dto.ConversationId)).AttachmentPlaceholderCreated(dto);
    //     return dto; // includes presigned URL info if you want to return it via command instead of client calling another endpoint
    // }
    //
    // public async Task AttachmentLink(AttachmentLinkCommand cmd, CancellationToken ct = default)
    // {
    //     cmd.RequesterUserId ??= userAccessor.GetUserId();
    //     var result = await sender.Send(cmd, ct); // creates MediaFile, sets MediaFileId, BindingStatus=Linked
    //     await Clients.Group(CG(result.ConversationId)).AttachmentLinked(result);
    // }
    //
    // public async Task AttachmentMarkFailed(AttachmentFailCommand cmd, CancellationToken ct = default)
    // {
    //     cmd.RequesterUserId ??= userAccessor.GetUserId();
    //     var result = await sender.Send(cmd, ct); // sets BindingStatus=Failed, ErrorCode
    //     await Clients.Group(CG(result.ConversationId)).AttachmentFailed(result);
    // }
    
    // ----- V5 -----
    
    // public async Task StartTyping(int conversationId)
    // {
    //     await sender.Send(new EnsureMemberCommand(conversationId, userAccessor.GetUserId()));
    //     await Clients.OthersInGroup(CG(conversationId))
    //         .Typing(new TypingDto(conversationId, userAccessor.GetUserId(), true));
    // }
    //
    // public async Task StopTyping(int conversationId)
    // {
    //     await sender.Send(new EnsureMemberCommand(conversationId, userAccessor.GetUserId()));
    //     await Clients.OthersInGroup(CG(conversationId))
    //         .Typing(new TypingDto(conversationId, userAccessor.GetUserId(), false));
    // }
    
    // ----- V6 -----
    
    // [Authorize(Roles = AppRoles.Admin)]
    // public async Task AssignAgent(AssignAgentCommand cmd, CancellationToken ct = default)
    // {
    //     var dto = await sender.Send(cmd, ct);
    //     await Clients.Group(CG(dto.Id)).ConversationUpdated(dto);
    // }
    //
    // [Authorize(Roles = AppRoles.Admin)]
    // public async Task CloseConversation(int conversationId, CancellationToken ct = default)
    // {
    //     var dto = await sender.Send(new CloseConversationCommand(conversationId), ct);
    //     await Clients.Group(CG(dto.Id)).ConversationUpdated(dto);
    // }
    //
    // [Authorize(Roles = AppRoles.Admin)]
    // public async Task ReopenConversation(int conversationId, CancellationToken ct = default)
    // {
    //     var dto = await sender.Send(new ReopenConversationCommand(conversationId), ct);
    //     await Clients.Group(CG(dto.Id)).ConversationUpdated(dto);
    // }
}