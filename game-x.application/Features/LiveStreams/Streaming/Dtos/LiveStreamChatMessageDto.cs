namespace game_x.application.Features.LiveStreams.Streaming.Dtos;

public sealed class LiveStreamChatMessageDto
{
    public Guid Id { get; set; }
    public Guid StreamId { get; set; }
    public string StreamKey { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
    public bool IsHost { get; set; }
    public string Message { get; set; } = string.Empty;
    public LiveStreamChatMessageType MessageType { get; set; }
    public decimal? DonationAmount { get; set; }
    public bool IsDeleted { get; set; }
    public string? DeleteReason { get; set; }
    public DateTime SentAt { get; set; }
}
