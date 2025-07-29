using game_x.application.Contract.Infrastructure.Email;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Infrastructure.Services.VerificationCode;
using game_x.application.Services.Verification;

namespace game_x.application.Services.Notification;

public sealed class EmailVerificationService(
    IEmailService emailService,
    IVerificationCodeService verificationCodeService) : IEmailVerificationProcessor
{
    public void SendVerificationEmail(string email)
    {
        var code = verificationCodeService
            .GenerateCode(email, VerificationPurposes.EmailVerification, TimeSpan.FromMinutes(10));
        emailService.SendVerificationEmailAsync(email, code);
    }

    public bool VerifyEmail(string email, string code)
    {
        var result = verificationCodeService.VerifyCode(email, VerificationPurposes.EmailVerification, code);
        return result;
    }
}
