namespace game_x.application.Features.Chat.Dtos;

public sealed record ConversationDto(
    Guid ConversationId,
    string? GuestId,
    ConversationStatus Status,
    ConversationType Type,
    RoleInConversation? LastSenderRole,
    string? CustomerId,
    string? CustomerDisplayName,
    string? CustomerAvatarUrl,
    string LastUserId,
    string LastUserName,
    string? LastUserAvatarUrl,
    DateTime? LastResolvedAt,
    int? LastResolvedMessageId,
    DateTime LastMessageAt,
    Guid LastMessageId,
    int? LastMessageIndex,
    string LastMessageText,
    MessageKind LastMessageKind,
    int? UnreadCount,
    bool? IsHidden);