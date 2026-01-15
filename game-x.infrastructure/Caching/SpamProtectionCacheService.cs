using game_x.application.Contract.Infrastructure.Caching;
using game_x.share.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.Caching;

public sealed class SpamProtectionCacheService(
    IMemoryCache cache,
    IOptions<SpamProtectionSettings> spamSetting) : CacheService(cache), ISpamProtectionCacheService
{
    #region ■ Resend limiter
    private static string GetResendKey(string email) => $"spam-protection:verify-code:resend:{email}";

    public Task<bool> CanResendVerifyCodeAsync(string email)
    {
        return Task.FromResult(!TryGetValue<DateTimeOffset>(GetResendKey(email), out _));
    }

    public Task<TimeSpan?> GetResendWaitTimeAsync(string email)
    {
        if (TryGetValue<DateTimeOffset>(GetResendKey(email), out var timestamp))
        {
            var remaining = timestamp - DateTimeOffset.UtcNow;
            return Task.FromResult<TimeSpan?>(remaining > TimeSpan.Zero ? remaining : null);
        }

        return Task.FromResult<TimeSpan?>(null);
    }

    public Task SetResendCooldownAsync(string email, TimeSpan? duration = null)
    {
        var key = GetResendKey(email);
        var expiry = DateTimeOffset.UtcNow.Add(duration ?? spamSetting.Value.VerifyEmailCooldown);

        Set(key, expiry, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = duration ?? spamSetting.Value.VerifyEmailCooldown
        });

        return Task.CompletedTask;
    }
    #endregion

    #region ■ Verify attempt limiter
    private static string GetAttemptKey(string email) => $"spam-protection:verify-code:attempt:{email}";

    public Task<bool> IsVerifyLockedAsync(string email)
    {
        if (TryGetValue<AttemptInfo>(GetAttemptKey(email), out var info))
        {
            if (info!.FailedCount >= spamSetting.Value.MaxVerifyAttempts
                && info.LockedAt.HasValue
                && info.LockedAt.Value.Add(spamSetting.Value.VerifyLockDuration) > DateTimeOffset.UtcNow)
                return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<TimeSpan?> GetVerifyRetryAfterAsync(string email)
    {
        if (TryGetValue<AttemptInfo>(GetAttemptKey(email), out var info)
            && info!.LockedAt.HasValue)
        {
            var remaining = info.LockedAt.Value.Add(spamSetting.Value.VerifyLockDuration) - DateTimeOffset.UtcNow;
            return Task.FromResult<TimeSpan?>(remaining > TimeSpan.Zero ? remaining : null);
        }

        return Task.FromResult<TimeSpan?>(null);
    }

    public Task RegisterVerifyFailureAsync(string email)
    {
        var key = GetAttemptKey(email);
        AttemptInfo entry;

        if (!TryGetValue<AttemptInfo>(key, out var current))
        {
            entry = new AttemptInfo(1, null);
        }
        else
        {
            var failedCount = current!.FailedCount + 1;
            var lockedAt = failedCount >= spamSetting.Value.MaxVerifyAttempts ? DateTimeOffset.UtcNow : current.LockedAt;
            entry = new AttemptInfo(failedCount, lockedAt);
        }

        Set(key, entry, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = spamSetting.Value.VerifySlidingWindow
        });

        return Task.CompletedTask;
    }

    public Task ResetVerifyAttemptAsync(string email)
    {
        Remove(GetAttemptKey(email));
        return Task.CompletedTask;
    }

    private record AttemptInfo(int FailedCount, DateTimeOffset? LockedAt);
    #endregion
}
