using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Streaming.Dtos;

public sealed class LiveStreamDonationDto
{
    public Guid Id { get; set; }
    public Guid LivestreamScheduleId { get; set; }
    public string DonorId { get; set; } = string.Empty;
    public string DonorName { get; set; } = string.Empty;
    public Guid? GiftId { get; set; }
    [JsonIgnore]
    public MediaFile? Animation { get; set; }
    public string? AnimationUrl { get; set; }
    public string? Message { get; set; }
    public decimal Amount { get; set; }
    public DateTime DonatedAt { get; set; }
}
