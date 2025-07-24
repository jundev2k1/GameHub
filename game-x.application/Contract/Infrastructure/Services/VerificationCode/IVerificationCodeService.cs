namespace game_x.application.Contract.Infrastructure.Services.VerificationCode;

public interface IVerificationCodeService
{
    Task<string> GenerateCodeAsync(string email, string purpose, TimeSpan? expiresIn = null);
    Task<bool> VerifyCodeAsync(string email, string purpose, string inputCode);
}