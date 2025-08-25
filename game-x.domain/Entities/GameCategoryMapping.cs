namespace game_x.domain.Entities;

public sealed class GameCategoryMapping : BaseEntity<object>
{
    public int GameId { get; private set; }
    public Game Game { get; private set; } = default!;

    public int CategoryId { get; private set; }
    public GameCategory Category { get; private set; } = default!;

    public bool IsPrimary { get; private set; } = false;
    public int Priority { get; private set; } = 0;

    public static GameCategoryMapping Create(Game game, GameCategory category, bool isPrimary = false, int priority = 0)
    {
        if (game == null)
            throw new ArgumentNullException(nameof(game), "Game cannot be null.");
        if (category == null)
            throw new ArgumentNullException(nameof(category), "Category cannot be null.");

        return new GameCategoryMapping
        {
            Game = game,
            Category = category,
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
