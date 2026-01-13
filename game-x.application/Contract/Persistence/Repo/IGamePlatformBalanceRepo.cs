namespace game_x.application.Contract.Persistence.Repo;

public interface IGamePlatformBalanceRepo
{
    Task<IEnumerable<GamePlatformBalance>> GetBalancesByUserIdAsync(string userId, CancellationToken ct = default);

    Task<GamePlatformBalance?> GetByPlatformIdAsync(string userId, Guid platformId, CancellationToken ct = default);

    Task SyncOrCreateAsync(
        string userId,
        int platformId,
        decimal availableBalance,
        decimal lockedBalance,
        CancellationToken ct = default);
}
