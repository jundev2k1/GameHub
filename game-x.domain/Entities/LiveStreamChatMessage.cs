namespace game_x.domain.Entities;

public sealed class LiveStreamChatMessage : BaseEntity<int>
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public int LiveStreamId { get; private set; }
    public LivestreamSchedule LiveStream { get; private set; } = default!;
    public string SenderId { get; private set; } = string.Empty;
    public User Sender { get; private set; } = default!;
    public string Message { get; private set; } = string.Empty;
    public LiveStreamChatMessageType MessageType { get; private set; }
    public decimal? DonationAmount { get; private set; }
    public bool IsDeleted { get; private set; } = false;
    public string? DeleteReason { get; private set; }
    public DateTime SentAt { get; private set; }

    public static LiveStreamChatMessage Create(
        string senderId,
        string message,
        LiveStreamChatMessageType messageType,
        decimal? donationAmount = null)
    {
        if (string.IsNullOrWhiteSpace(senderId))
            throw new ArgumentException("SenderId cannot be null or empty.", nameof(senderId));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or empty.", nameof(message));

        if (messageType == LiveStreamChatMessageType.Donation)
        {
            if (donationAmount == null || donationAmount <= 0)
                throw new ArgumentException("Donation amount must be greater than zero for donation messages.", nameof(donationAmount));
        }

        return new LiveStreamChatMessage
        {
            SenderId = senderId,
            Message = message,
            MessageType = messageType,
            DonationAmount = donationAmount,
            SentAt = DateTime.UtcNow
        };
    }

    public void MarkAsDeleted(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason for deletion cannot be null or empty.", nameof(reason));

        IsDeleted = true;
        DeleteReason = reason;
    }
}
