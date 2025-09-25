namespace game_x.application.Features.Chat.Dtos;

public sealed record ListedConversationDto(
    Guid ConversationId,
    ConversationType Type,
    ConversationStatus Status,
    string LastUserId,
    string LastUserName,
    string? LastUserAvatarUrl,
    RoleInConversation LastSenderRole,
    DateTime LastMessageAt,
    Guid LastMessageId,
    string LastMessageText,
    MessageKind LastMessageKind );