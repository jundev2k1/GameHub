namespace game_x.domain.Entities;

public sealed class GamePlatform : BaseEntity<int>
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<Game> Games { get; private set; } = default!;
    public ICollection<GameTransaction> GameTransactions { get; private set; } = default!;

    public static GamePlatform Create(string name, string desc, string note, int priority, Guid? publicId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return new GamePlatform
        {
            PublicId = publicId ?? Guid.NewGuid(),
            Name = name,
            Description = desc,
            Note = note,
            Priority = priority,
        };
    }

    public void SetActive(bool isActive) =>
        IsActive = isActive;
}
