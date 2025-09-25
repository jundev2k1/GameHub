namespace game_x.application.Contract.Infrastructure.Caching;

public interface IUserCacheService
{
    Task RefreshInactiveUser(CancellationToken ct = default);

    Task<string[]> GetInactiveUserIds(CancellationToken ct = default);

    Task<bool> IsInactiveUser(string userId, CancellationToken ct = default);
}