namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public sealed record InboxUpsertSignalDto(
    Guid ConversationId,
    ConversationStatus Status,
    MessageKind LastMessageKind,
    DateTime LastMessageAt,
    Guid LastMessageId,
    string LastMessageText);