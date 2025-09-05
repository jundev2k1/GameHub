namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
    
public sealed record ConversationSignalDto(
    Guid ConversationId,
    string GuestId,
    ConversationStatus Status,
    string? CustomerId,
    string? CustomerDisplayName,
    string? CustomerAvatarUrl,
    DateTime LastMessageAt,
    Guid LastMessageId,
    string LastMessagePreview
);