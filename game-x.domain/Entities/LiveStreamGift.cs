namespace game_x.domain.Entities;

public sealed class LiveStreamGift : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public int? IconId { get; private set; }
    public MediaFile? Icon { get; private set; } = default!;
    public int? AnimationId { get; private set; }
    public MediaFile? Animation { get; private set; } = default!;
    public decimal CoinCost { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;

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

    public void UpdateIcon(MediaFile image)
    {
        Icon = image;
    }

    public void UpdateAnimation(MediaFile file)
    {
        Animation = file;
    }

    public void Enable() => IsActive = true;

    public void Disable() => IsActive = false;

    public void Delete() => IsDeleted = true;
}
