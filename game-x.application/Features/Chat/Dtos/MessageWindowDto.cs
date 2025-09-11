namespace game_x.application.Features.Chat.Dtos;

public sealed record MessageWindowDto
{
    public List<ListedMessageDto> Items { get; init; } = new();
    public int AnchorMessageId { get; init; }
    public bool HasMoreBefore { get; init; }
    public bool HasMoreAfter { get; init; }
    public string? PrevCursor { get; init; }
    public string? NextCursor { get; init; }
}