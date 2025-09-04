namespace game_x.domain.Entities;

public sealed class GameTag : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public GameTagIcon Icon { get; private set; } = default!;
    public GameTagColor Color { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    public ICollection<GameTagMapping> GameTagMappings { get; private set; } = default!;

    public static GameTag Create(
        string name,
        string desc,
        GameTagIcon icon,
        GameTagColor color,
        string note)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return new GameTag
        {
            Name = name,
            Description = desc,
            Icon = icon,
            Color = color,
            Note = note
        };
    }

    public void Update(
        string? name = null,
        string? desc = null,
        GameTagIcon? icon = null,
        GameTagColor? color = null,
        string? note = null)
    {
        Name = name ?? Name;
        Description = desc ?? Description;
        Icon = icon ?? Icon;
        Color = color ?? Color;
        Note = note ?? Note;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
