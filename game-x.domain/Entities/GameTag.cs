namespace game_x.domain.Entities;

public sealed class GameTag : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public ICollection<GameTagMapping> GameTagMappings { get; private set; } = default!;

    public static GameTag Create(string name, string desc, string note)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return new GameTag
        {
            Name = name,
            Description = desc,
            Note = note
        };
    }

    public void Update(string name, string? desc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        Name = name;
        Description = desc ?? Description;
    }
}
