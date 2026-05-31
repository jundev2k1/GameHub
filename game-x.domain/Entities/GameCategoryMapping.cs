namespace game_x.domain.Entities;

public sealed class GameCategoryMapping : BaseEntity<object>
{
    public int GameId { get; private set; }
    public Game Game { get; private set; } = default!;

    public int CategoryId { get; private set; }
    public GameCategory Category { get; private set; } = default!;

    public bool IsPrimary { get; private set; } = false;
    public int Priority { get; private set; } = 0;

    public static GameCategoryMapping Create(int gameId, int categoryId, bool isPrimary = false, int priority = 0)
    {
        return new GameCategoryMapping
        {
            GameId = gameId,
            CategoryId = categoryId,
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
