using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Gifts.Dtos;

public class LiveStreamGiftPriceDto
{
    [JsonIgnore]
    public int CryptoTokenLocalId { get; set; }
    public Guid CryptoTokenId { get; set; }
    [JsonIgnore]
    public int LiveStreamGiftLocalId { get; set; }
    public Guid LiveStreamGiftId { get; set; }
    public decimal TokenCost { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
