namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public sealed record MessageWindowDto
{
    public List<ListMessageDto> Items { get; init; } = new();
    public int AnchorMessageId { get; init; }
    public bool HasMoreBefore { get; init; }
    public bool HasMoreAfter { get; init; }
    public string? PrevCursor { get; init; }
    public string? NextCursor { get; init; }
}