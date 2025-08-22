namespace game_x.domain.Entities;

public sealed class GameCategory : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<Game> Games { get; private set; } = default!;

    public static GameCategory Create(string name, string desc, string note, int priority)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return new GameCategory
        {
            Name = name,
            Description = desc,
            Note = note,
            Priority = priority,
        };
    }
}
