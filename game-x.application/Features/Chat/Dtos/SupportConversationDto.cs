namespace game_x.application.Features.Chat.Dtos;

public sealed record SupportConversationDto(
    Guid ConversationId,
    ConversationStatus Status,
    string GuestId,
    string CustomerId,
    string CustomerDisplayName,
    string? CustomerAvatarUrl,
    string LastUserId,
    string LastUserName,
    RoleInConversation LastSenderRole,
    DateTime LastMessageAt,
    Guid LastMessageId,
    string LastMessageText,
    MessageKind LastMessageKind);