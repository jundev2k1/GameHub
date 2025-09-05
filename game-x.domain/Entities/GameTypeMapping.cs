namespace game_x.domain.Entities;

public sealed class GameTypeMapping : BaseEntity<object>
{
    public int GameId { get; private set; }
    public Game Game { get; private set; } = default!;

    public int TypeId { get; private set; }
    public GameType Type { get; private set; } = default!;

    public bool IsPrimary { get; private set; } = false;
    public int Priority { get; private set; } = 0;

    public static GameTypeMapping Create(
        int gameId,
        int typeId,
        bool isPrimary = false,
        int priority = 0)
    {
        if (priority < 0)
            throw new ArgumentException("Priority cannot be negative.", nameof(priority));

        return new GameTypeMapping
        {
            GameId = gameId,
            TypeId = typeId,
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
            throw new ArgumentException("Priority cannot be negative.", nameof(priority));
        Priority = priority;
    }
}
