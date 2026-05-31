namespace game_x.domain.Entities;

public sealed class GameTagMapping : BaseEntity<int>
{
    public int GameId { get; private set; }
    public Game Game { get; private set; } = default!;
    public int TagId { get; private set; }
    public GameTag Tag { get; private set; } = default!;

    public bool IsPrimary { get; private set; } = false;
    public int Priority { get; private set; } = 0;

    public static GameTagMapping Create(
        int gameId,
        int tagId,
        bool isPrimary = false,
        int priority = 0)
    {
        if (priority < 0)
            throw new ArgumentException("Priority must be greater than or equal to 0.", nameof(priority));

        return new GameTagMapping
        {
            GameId = gameId,
            TagId = tagId,
            IsPrimary = isPrimary,
            Priority = priority,
        };
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }
}
