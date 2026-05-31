namespace game_x.application.Contract.Infrastructure.Services.VerificationCode;

public interface IVerificationCodeService
{
    string GenerateCode(string email, string purpose, TimeSpan? expiresIn = null);

    bool VerifyCode(string email, string purpose, string inputCode);
}
