namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public sealed record MessageHitDto
{
    public int MessageId { get; init; }
    public DateTime SentAt { get; init; } // UTC
    public string Snippet { get; init; } = "";
}