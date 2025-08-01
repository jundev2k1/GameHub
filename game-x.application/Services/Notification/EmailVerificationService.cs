using game_x.application.Contract.Infrastructure.Email;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Infrastructure.Services.VerificationCode;
using game_x.application.Services.Verification;

namespace game_x.application.Services.Notification;

public sealed class EmailVerificationService(
    IEmailService emailService,
    IVerificationCodeService verificationCodeService) : IEmailVerificationProcessor
{
    public void SendVerificationEmail(string email, string purpose)
    {
        var code = verificationCodeService
            .GenerateCode(email, purpose, TimeSpan.FromMinutes(10));
        emailService.SendVerificationEmailAsync(email, code);
    }

    public bool VerifyEmail(string email, string code, string purpose)
    {
        var result = verificationCodeService.VerifyCode(email, purpose, code);
        return result;
    }
}
