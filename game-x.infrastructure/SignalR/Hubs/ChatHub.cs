using System.Text.Json;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Features.Chat.Commands.SendSupportMessage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace game_x.infrastructure.SignalR.Hubs;

public interface IChatClient
{
    // ----- V1 -----
    Task ConversationCreated(ConversationDto dto);
    Task MemberAdded(ConversationMemberDto dto);
    Task MessageCreated(MessageDto dto);
    
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

[Authorize(Roles = $"{AppRoles.User},{AppRoles.Admin},{AppRoles.Cs},{AppRoles.Root}")]
public sealed class ChatHub(
    ISender sender,
    IUserAccessor userAccessor,
    ILogger<ChatHub> logger) : Hub<IChatClient>
{
    public const string Path = "/hubs/chat";

    // public override async Task OnConnectedAsync()
    // {
    //     var userId = Context.UserIdentifier ?? userAccessor.GetUserId();
    //     await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Member(userId));
    //     logger.LogInformation("ChatHub connected: {UserId}", userId);
    //     await base.OnConnectedAsync();
    // }

    public override async Task OnConnectedAsync()
    {
        var userId = userAccessor.GetUserId();
        var role = userAccessor.GetRoles();
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Member(userId));
        
        // If the user is an Admin/Cs, also add them to role groups (so they get broadcasts)
        if (Context.User?.IsInRole(AppRoles.Admin) == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Role(AppRoles.Admin));
        if (Context.User?.IsInRole(AppRoles.Cs) == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupNames.Role(AppRoles.Cs));

        logger.LogInformation("ChatHub {Role} connected: {UserId}",role.ToString(), userId);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        var userId = userAccessor.GetUserId();
        var role = userAccessor.GetRoles();
        logger.LogInformation("ChatHub {Role} disconnected: {UserId}",role.ToString(), userId);
        await base.OnDisconnectedAsync(ex);
    }

    public Task<string> Ping(dynamic payload) => Task.FromResult($"pong:{JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true })}");
    
    // --- Customer Support ---
    
    /// <summary>
    /// Send a support message. Creates the user's support conversation if missing
    /// and broadcasts to the conversation AND to all Admin/Cs role groups.
    /// </summary>
    public async Task<MessageDto> SendSupportMessage(SendSupportMessageCommand cmd)
    {
        var ct = Context.ConnectionAborted;
        
        var result = await sender.Send(cmd, ct);
        // If a new conversation was created on first sending, notify the sender (optional)
        if (result.CreatedConversation is { } conv)
        {
            await Clients.Caller.ConversationCreated(conv);
        }

        // Broadcast to conversation participants (user’s devices)…
        await Clients.Group(GroupNames.Conversation(result.Message.ConversationId)).MessageCreated(result.Message);
        await Clients.Group(GroupNames.Role(AppRoles.Admin)).MessageCreated(result.Message);
        await Clients.Group(GroupNames.Role(AppRoles.Cs)).MessageCreated(result.Message);

        return result.Message;
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