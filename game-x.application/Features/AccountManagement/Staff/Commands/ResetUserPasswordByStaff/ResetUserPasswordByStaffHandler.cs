using game_x.application.Contract.Infrastructure.Services.VerificationCode;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Services.Verification;

namespace game_x.application.Features.AccountManagement.Staff.Commands.ResetUserPasswordByStaff;

public sealed class ResetUserPasswordByStaffHandler(
    IVerificationCodeService vcService,
    IAuthService authService,
    IUserRepo userRepo) : ICommandHandler<ResetUserPasswordByStaffCommand>
{
    public async Task<Unit> Handle(ResetUserPasswordByStaffCommand request, CancellationToken ct)
    {
        var isValid = await vcService.VerifyCodeAsync(
            request.Email,
            VerificationPurposes.EmailVerification,
            request.VerificationCode);
        if (!isValid) throw new BadRequestException(MessageCode.System.InvalidVerifyCode);

        var user = await userRepo.GetUserByEmailAsync(request.Email, ct);
        var token = await authService.GeneratePasswordResetTokenAsync(user, ct);

        await authService.ResetPasswordAsync(user, token, request.NewPassword, ct);
        return Unit.Value;
    }
}
