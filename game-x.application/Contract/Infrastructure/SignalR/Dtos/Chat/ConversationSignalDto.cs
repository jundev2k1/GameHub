namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
    
public sealed record ConversationSignalDto(
    Guid ConversationId,
    string? GuestId,
    ConversationStatus Status,
    ConversationType Type,
    RoleInConversation LastSenderRole,
    string? CustomerId,
    string? CustomerDisplayName,
    string? CustomerAvatarUrl,
    string LastUserId,
    string LastUserName,
    string? LastUserAvatarUrl,
    DateTime LastMessageAt,
    Guid LastMessageId,
    string LastMessageText,
    MessageKind LastMessageKind,
    int? BackOfficeUnreadCount,
    int? ClientUnreadCount);