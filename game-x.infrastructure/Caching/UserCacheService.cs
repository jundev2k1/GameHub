using game_x.application.Contract.Infrastructure.Caching;
using game_x.domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class UserCacheService(IMemoryCache cache, GameXContext context)
    : CacheService(cache), IUserCacheService
{
    private const string PrefixModule = "user:";
    private const string InactiveUserIdsCacheKey = $"{PrefixModule}inactive-user-list:today";

    public async Task RefreshInactiveUser(CancellationToken ct = default)
    {
        var inActiveUsers = await context.Users
            .Where(u => u.IsDeleted || u.Status == AppUserStatus.Inactive)
            .Select(u => u.Id)
            .ToArrayAsync(ct);
        Set(InactiveUserIdsCacheKey, inActiveUsers);
    }

    public async Task<string[]> GetInactiveUserIds(CancellationToken ct = default)
    {
        if (this.InactiveUserIds is null) await RefreshInactiveUser(ct);

        return this.InactiveUserIds ?? Array.Empty<string>();
    }

    public async Task<bool> IsInactiveUser(string userId, CancellationToken ct = default)
    {
        var inactiveUserIds = await GetInactiveUserIds(ct);
        return inactiveUserIds.Contains(userId);
    }

    private string[]? InactiveUserIds => Get<string[]>(InactiveUserIdsCacheKey);
}
