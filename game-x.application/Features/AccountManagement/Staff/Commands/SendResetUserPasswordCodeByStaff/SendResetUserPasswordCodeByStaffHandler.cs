using game_x.application.Contract.Infrastructure.Email;
using game_x.application.Contract.Infrastructure.Services.VerificationCode;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Services.Verification;

namespace game_x.application.Features.AccountManagement.Staff.Commands.SendResetUserPasswordCodeByStaff;

public sealed class SendResetPasswordCodeCommandHandler(
    IUserRepo userRepo,
    IVerificationCodeService verificationCodeService,
    IEmailService emailService) : ICommandHandler<SendResetUserPasswordCodeByStaffCommand>
{
    public async Task<Unit> Handle(SendResetUserPasswordCodeByStaffCommand request, CancellationToken ct = default)
    {
        await userRepo.GetUserByEmailAsync(request.Email, ct);
        var code = await verificationCodeService.GenerateCodeAsync(
            request.Email,
            VerificationPurposes.EmailVerification, TimeSpan.FromMinutes(10));
        await emailService.SendResetPasswordEmailAsync(request.Email, code);

        return Unit.Value;
    }
}
