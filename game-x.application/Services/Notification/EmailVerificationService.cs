using game_x.application.Contract.Infrastructure.Email;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Infrastructure.Services.VerificationCode;
using game_x.application.Services.Verification;

namespace game_x.application.Services.Notification;

public sealed class EmailVerificationService(IEmailService emailService, IVerificationCodeService verificationCodeService) : IEmailVerificationProcessor
{
    public async Task SendVerificationEmailAsync(string email, CancellationToken ct = default)
    {
        var code = await verificationCodeService
            .GenerateCodeAsync(email, VerificationPurposes.EmailVerification, TimeSpan.FromMinutes(10));
        await emailService.SendVerificationEmailAsync(email, code);
    }
}
