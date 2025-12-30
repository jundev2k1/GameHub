namespace game_x.application.Features.Chat.Dtos;

public record DeletedMessageDto(
    Guid Id,
    Guid ConversationId,
    ConversationType ConversationType);