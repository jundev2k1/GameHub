namespace game_x.domain.Entities;

public sealed class GameRecommendItem : BaseEntity<int>, IAuditable
{
    public int GameRecommendId { get; private set; }
    public GameRecommend GameRecommend { get; private set; } = default!;

    public int GameId { get; private set; }
    public Game Game { get; private set; } = default!;

    public int Priority { get; private set; }
    public string? CustomTitle { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static GameRecommendItem Create(
        int recommendGroupId,
        int gameId,
        int priority = 0,
        string? customTitle = null,
        bool isActive = true)
    {
        if (priority < 0)
            throw new ArgumentException("Priority must be greater than or equal 0.", nameof(priority));

        return new GameRecommendItem
        {
            GameRecommendId = recommendGroupId,
            GameId = gameId,
            Priority = priority,
            CustomTitle = customTitle,
            IsActive = isActive
        };
    }

    public void SetPriority(int priority)
    {
        if (priority < 0)
            throw new ArgumentException("Priority must be greater than or equal 0.", nameof(priority));

        Priority = priority;
    }

    public void UpdateDisplay(string? customTitle)
    {
        CustomTitle = customTitle;
    }

    public void SetActive(bool isActive) => IsActive = isActive;
}
