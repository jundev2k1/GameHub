namespace game_x.application.Features.Chat.Dtos;

public sealed record ConvUnreadDto
{
    public Guid ConversationId { get; init; }
    public ConversationType Type { get; init; }
    public ConversationStatus Status { get; init; }
    public int Unread { get; init; }
    public int? Read { get; init; }
}