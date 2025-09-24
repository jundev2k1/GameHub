namespace game_x.application.Features.LiveStreams.Gifts.Dtos;

public class LiveStreamGiftDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal CoinCost { get; set; }
    public int Priority { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
