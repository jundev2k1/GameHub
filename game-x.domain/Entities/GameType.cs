namespace game_x.domain.Entities;

public sealed class GameType : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<GameTypeMapping> GameTypeMappings { get; private set; } = default!;

    public static GameType Create(string name, string desc, string note, int priority, Guid? publicId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return new GameType
        {
            PublicId = publicId ?? Guid.NewGuid(),
            Name = name,
            Description = desc,
            Note = note,
            Priority = priority,
        };
    }
}
