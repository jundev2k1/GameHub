namespace game_x.domain.Entities;

public sealed class GameTypeMapping : BaseEntity<object>
{
    public int GameId { get; private set; }
    public Game Game { get; private set; } = default!;

    public int TypeId { get; private set; }
    public GameType Type { get; private set; } = default!;

    public bool IsPrimary { get; private set; } = false;
    public int Priority { get; private set; } = 0;

    public static GameTypeMapping Create(Game game, GameType type, bool isPrimary = false, int priority = 0)
    {
        if (game == null)
            throw new ArgumentNullException(nameof(game), "Game cannot be null.");
        if (type == null)
            throw new ArgumentNullException(nameof(type), "Game Type cannot be null.");

        return new GameTypeMapping
        {
            Game = game,
            Type = type,
            IsPrimary = isPrimary,
            Priority = priority,
        };
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }

    public void SetPriority(int priority)
    {
        if (priority < 0)
            throw new ArgumentOutOfRangeException(nameof(priority), "Priority cannot be negative.");
        Priority = priority;
    }
}
