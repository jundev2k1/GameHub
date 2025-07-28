namespace game_x.application.Contract.Infrastructure.Caching;

public interface ISpamProtectionCacheService
{
    // -- Resend limiter --
    Task<bool> CanResendVerifyCodeAsync(string email);

    Task<TimeSpan?> GetResendWaitTimeAsync(string email);

    Task SetResendCooldownAsync(string email, TimeSpan duration);

    // -- Verify attempt limiter --
    Task<bool> IsVerifyLockedAsync(string email);

    Task RegisterVerifyFailureAsync(string email);

    Task ResetVerifyAttemptAsync(string email);

    Task<TimeSpan?> GetVerifyRetryAfterAsync(string email);
}
