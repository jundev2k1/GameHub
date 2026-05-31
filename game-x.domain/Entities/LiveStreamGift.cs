namespace game_x.domain.Entities;

public sealed class LiveStreamGift : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    public string Name { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public int? IconId { get; private set; }
    public MediaFile? Icon { get; private set; } = default!;
    public int? AnimationId { get; private set; }
    public MediaFile? Animation { get; private set; } = default!;
    public int? AnimationDuration { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;

    public ICollection<LiveStreamGiftPrice> GiftPrices { get; private set; } = [];

    public static LiveStreamGift Create(
        string name,
        string? notes,
        int priority,
        List<LiveStreamGiftPrice> giftPrices)
    {
        return new LiveStreamGift
        {
            Name = name,
            Notes = notes,
            Priority = priority,
            GiftPrices = giftPrices,
        };
    }

    public void Update(
        string name,
        string? notes,
        int priority,
        List<LiveStreamGiftPrice> giftPrices)
    {
        Name = name;
        Notes = notes;
        Priority = priority;
        GiftPrices = giftPrices;
    }

    public void UpdateIcon(MediaFile image)
    {
        Icon = image;
    }

    public void UpdateAnimation(MediaFile file, int duration = 1000)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(duration, 0);

        Animation = file;
        AnimationDuration = duration;
    }

    public void UpdateAnimationDuration(int duration)
    {
        if (!AnimationId.HasValue)
            throw new ArgumentException(nameof(AnimationId));

        ArgumentOutOfRangeException.ThrowIfLessThan(duration, 0);

        AnimationDuration = duration;
    }

    public void Enable() => IsActive = true;

    public void Disable() => IsActive = false;

    public void Delete() => IsDeleted = true;
}
