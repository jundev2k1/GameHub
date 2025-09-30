namespace game_x.application.Features.Chat.Dtos;

public sealed record ConvUnreadDto(Guid ConversationId, int Unread);