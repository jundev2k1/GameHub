namespace game_x.domain.Entities;

public sealed class GameTagMapping : BaseEntity<int>
{
    public int GameId { get; private set; }
    public Game Game { get; private set; } = default!;
    public int TagId { get; private set; }
    public GameTag Tag { get; private set; } = default!;

    public bool IsPrimary { get; private set; } = false;
    public int Priority { get; private set; } = 0;

    public static GameTagMapping Create(Game game, GameTag tag, int priority)
    {
        return new GameTagMapping
        {
            Game = game,
            Tag = tag,
            Priority = priority,
        };
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }
}
