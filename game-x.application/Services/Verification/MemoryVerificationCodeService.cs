using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Services.VerificationCode;

namespace game_x.application.Services.Verification;

public static class VerificationPurposes
{
    public const string EmailVerification = "email-verification";
    public const string ForgotPassword = "forgot-password";
    public const string ChangePassword = "change-password";
}

public sealed class MemoryVerificationCodeService(IEmailCacheService emailCache) : IVerificationCodeService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(10);

    public string GenerateCode(string email, string purpose, TimeSpan? expiresIn = null)
    {
        var randomCode = new Random()
            .Next(0, 100000000)
            .ToString("D8");
        emailCache.SetCode(email, purpose, randomCode, expiresIn ?? DefaultExpiration);
        return randomCode;
    }

    public bool VerifyCode(string email, string purpose, string inputCode)
    {
        var storedCode = emailCache.GetCode(email, purpose);
        if (storedCode == inputCode)
        {
            emailCache.RemoveCode(email, purpose);
            return true;
        }

        return false;
    }
}
