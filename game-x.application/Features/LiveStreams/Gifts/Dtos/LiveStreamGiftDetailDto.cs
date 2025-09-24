namespace game_x.application.Features.LiveStreams.Gifts.Dtos;

public sealed class LiveStreamGiftDetailDto : LiveStreamGiftDto
{
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
    public int GiftCount { get; set; }
    public decimal TotalCoinCost { get; set; }
}
