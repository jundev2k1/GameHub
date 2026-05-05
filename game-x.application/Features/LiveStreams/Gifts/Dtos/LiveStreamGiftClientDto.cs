using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Gifts.Dtos;

public class LiveStreamGiftClientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [JsonIgnore]
    public int? IconId { get; set; } = default!;
    public string? IconUrl { get; set; }
    [JsonIgnore]
    public int? AnimationId { get; set; } = default!;
    public string? AnimationUrl { get; set; }
    public int? AnimationDuration { get; set; }
	public int Priority { get; set; }
	public LiveStreamGiftPriceClientDto[] GiftPrices { get; set; } = [];
}
