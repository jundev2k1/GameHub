namespace game_x.domain.Entities;

public sealed class GamePlatform : BaseEntity<int>
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;
    public ICollection<Game> Games { get; private set; } = null!;
    public ICollection<TransactionExternal> TransactionExternals { get; private set; } = null!;

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

    public void Update(string name, string desc, string note, int priority, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        if (priority < 0)
            throw new ArgumentException("Priority must be greater than or equal 0.", nameof(priority));

        Name = name;
        Description = desc;
        Note = note;
        Priority = priority;
        IsActive = isActive;
    }

    public void SetActive(bool isActive) =>
        IsActive = isActive;
}
