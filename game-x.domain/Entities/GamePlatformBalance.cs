namespace game_x.domain.Entities;

public sealed class GamePlatformBalance : BaseEntity<int>
{
    public string UserId { get; private set; } = string.Empty!;
    public int PlatformId { get; private set; }
    public GamePlatform Platform { get; private set; } = default!;

    public decimal AvailableBalance { get; private set; }
    public decimal LockedBalance { get; private set; }
    public DateTime LastSyncedAt { get; private set; }

    public static GamePlatformBalance Create(string userId, int platformId)
    {
        return new GamePlatformBalance
        {
            UserId = userId,
            PlatformId = platformId,
            AvailableBalance = 0,
            LockedBalance = 0,
            LastSyncedAt = DateTime.UtcNow,
        };
    }

    public (decimal AvailableBalance, decimal LockedBalance, decimal TotalBalance) GetBalance()
    {
        var totalBalance = AvailableBalance + LockedBalance;
        return (AvailableBalance, LockedBalance, totalBalance);
    }

    public void SyncBalance(decimal balance, decimal lockedBalance)
    {
        AvailableBalance = balance;
        LockedBalance = lockedBalance;
        LastSyncedAt = DateTime.UtcNow;
    }
}
