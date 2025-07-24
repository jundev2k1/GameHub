using game_x.application.Contract.Infrastructure.Services.VerificationCode;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.application.Services.Verification;

public static class VerificationPurposes
{
    public const string EmailVerification = "email-verification";
    public const string ForgotPassword = "forgot-password"; // Not logged in, use Email
}

public class MemoryVerificationCodeService(IMemoryCache cache) : IVerificationCodeService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(10);

    public Task<string> GenerateCodeAsync(string email, string purpose, TimeSpan? expiresIn = null)
    {
        var code = new Random().Next(100000, 999999).ToString();
        var cacheKey = GetCacheKey(email, purpose);

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiresIn ?? DefaultExpiration
        };

        cache.Set(cacheKey, code, options);
        return Task.FromResult(code);
    }

    public Task<bool> VerifyCodeAsync(string email, string purpose, string inputCode)
    {
        var cacheKey = GetCacheKey(email, purpose);

        if (cache.TryGetValue(cacheKey, out string? storedCode) && storedCode == inputCode)
        {
            cache.Remove(cacheKey); // Remove after successful verification
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    private static string GetCacheKey(string userId, string purpose)
    {
        return $"{purpose}:{userId}";
    }
}