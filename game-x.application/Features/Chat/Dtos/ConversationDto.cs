namespace game_x.application.Features.Chat.Dtos;

public sealed record ConversationDto(
    Guid ConversationId,
    ConversationType Type,
    ConversationStatus Status,
    string LastUserId,
    string LastUserName,
    string? LastUserAvatarUrl,
    DateTime LastMessageAt,
    Guid LastMessageId,
    string LastMessagePreview
);