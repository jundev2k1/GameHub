namespace game_x.domain.Entities;

public sealed class Game : BaseEntity<int>
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string GameCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int PlatformId { get; private set; }
    public GamePlatform Platform { get; private set; } = default!;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;
    public ICollection<GameCategory> Categories { get; private set; } = default!;
    public ICollection<GameType> GameTypes { get; private set; } = default!;

    public static Game Create(string name, string gameCode, string desc, string note, int priority)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(gameCode, nameof(gameCode));

        return new Game
        {
            GameCode = gameCode,
            Name = name,
            Description = desc,
            Note = note,
            Priority = priority,
        };
    }
}
