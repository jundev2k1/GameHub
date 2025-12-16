namespace game_x.domain.Entities;

public sealed class UserGameSession : BaseEntity<int>, IAuditable
{
    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = default!;
    public int PlatformId { get; private set; }
    public GamePlatform Platform { get; private set; } = default!;
    public int? GameId { get; private set; }
    public Game? Game { get; private set; }

    public DateTime LoginAt { get; private set; }
    public decimal BalanceSnapshot { get; private set; }

    public static UserGameSession Create(
        string userId,
        int platformId,
        int? gameId = null)
    {
        return new UserGameSession
        {
            UserId = userId,
            PlatformId = platformId,
            GameId = gameId,
            LoginAt = DateTime.UtcNow
        };
    }
}
