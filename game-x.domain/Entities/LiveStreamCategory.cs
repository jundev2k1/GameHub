namespace game_x.domain.Entities;

public sealed class LiveStreamCategory : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<LiveStreamCategoryMapping> CategoryMappings { get; private set; } = default!;

    public static LiveStreamCategory Create(string name, string desc, string note, int priority)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return new LiveStreamCategory
        {
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
}
