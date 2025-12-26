namespace game_x.domain.Entities;

public sealed class UserGameSession : BaseEntity<int>
{
    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = default!;
    public int PlatformId { get; private set; }
    public GamePlatform Platform { get; private set; } = default!;
    public int? GameId { get; private set; }
    public Game? Game { get; private set; }
    public bool IsEnd { get; private set; }
    public decimal BalanceSnapshot { get; private set; }

    public ICollection<UserGameSessionConnection> Connections { get; private set; } = [];

    public static UserGameSession Create(
        string userId,
        int platformId,
        int? gameId = null,
        decimal balanceSnapshot = 0)
    {
        return new UserGameSession
        {
            UserId = userId,
            PlatformId = platformId,
            GameId = gameId,
            BalanceSnapshot = balanceSnapshot
        };
    }

    public void SetEndSession()
    {
        IsEnd = true;
    }

    public void RegisterConnection(UserGameSessionConnection connection)
        => Connections.Add(connection);
}
