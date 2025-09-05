namespace game_x.application.Features.Chat.Dtos;

public sealed record SupportConversationDto(
    Guid ConversationId,
    string GuestId,
    string CustomerId,
    string CustomerDisplayName,
    string? CustomerAvatarUrl,
    DateTime LastMessageAt,
    Guid LastMessageId,
    string LastMessagePreview
);