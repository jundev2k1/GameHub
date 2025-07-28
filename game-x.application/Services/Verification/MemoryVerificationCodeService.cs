using game_x.application.Contract.Infrastructure.Services.VerificationCode;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.application.Services.Verification;

public static class VerificationPurposes
{
    public const string EmailVerification = "email-verification";
    public const string ForgotPassword = "forgot-password";
}

public sealed class MemoryVerificationCodeService(IMemoryCache cache) : IVerificationCodeService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(10);

    public string GenerateCode(string email, string purpose, TimeSpan? expiresIn = null)
    {
        var code = new Random()
            .Next(0, 100000000)
            .ToString("D8");
        var cacheKey = GetCacheKey(email, purpose);
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiresIn ?? DefaultExpiration
        };

        cache.Set(cacheKey, code, options);
        return code;
    }

    public bool VerifyCode(string email, string purpose, string inputCode)
    {
        var cacheKey = GetCacheKey(email, purpose);
        if (cache.TryGetValue(cacheKey, out string? storedCode) && storedCode == inputCode)
        {
            cache.Remove(cacheKey);
            return true;
        }

        return false;
    }

    private static string GetCacheKey(string userId, string purpose)
    {
        return $"{purpose}:{userId}";
    }
}
