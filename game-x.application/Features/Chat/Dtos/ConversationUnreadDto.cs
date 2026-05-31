namespace game_x.application.Features.Chat.Dtos;

public record ConversationUnreadDto(ConversationStatus Status, int? UnreadCount);