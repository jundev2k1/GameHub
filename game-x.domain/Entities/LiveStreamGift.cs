namespace game_x.domain.Entities;

public sealed class LiveStreamGift : BaseEntity<int>, IAuditable
{
    public string Name { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public int? ImageId { get; private set; }
    public MediaFile? Image { get; private set; } = default!;
    public decimal CoinCost { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static LiveStreamGift Create(string name, string? notes, decimal coinCost, int priority)
    {
        return new LiveStreamGift
        {
            Name = name,
            Notes = notes,
            CoinCost = coinCost,
            Priority = priority,
        };
    }

    public void Update(string name, string? notes, decimal coinCost, int priority)
    {
        Name = name;
        Notes = notes;
        CoinCost = coinCost;
        Priority = priority;
    }

    public void UpdateImage(MediaFile image)
    {
        Image = image;
    }

    public void Enable() => IsActive = true;

    public void Disable() => IsActive = false;
}
