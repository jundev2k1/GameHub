namespace game_x.domain.Entities;

public sealed class LiveStreamDonation : BaseEntity<int>
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public int LivestreamScheduleId { get; private set; }
    public LivestreamSchedule LivestreamSchedule { get; private set; } = default!;
    public string DonorId { get; private set; } = string.Empty;
    public User Donor { get; private set; } = default!;
    public int? GiftId { get; private set; }
    public LiveStreamGift? Gift { get; private set; } = default!;

    public string? Message { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime DonatedAt { get; private set; } = DateTime.UtcNow;

    public static LiveStreamDonation Create(
        int livestreamScheduleId,
        string donorId,
        string message,
        decimal amount)
    {
        if (string.IsNullOrWhiteSpace(donorId))
            throw new ArgumentException("Donor id is required.", donorId);
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", amount.ToString());

        return new LiveStreamDonation
        {
            LivestreamScheduleId = livestreamScheduleId,
            DonorId = donorId,
            Message = message,
            Amount = amount,
            DonatedAt = DateTime.UtcNow
        };
    }

    public void SetGift(LiveStreamGift gift)
    {
        Gift = gift;
    }
}
