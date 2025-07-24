using game_x.application.Extensions;

namespace game_x.application.Features.AccountManagement.Staff.Commands.ResetUserPasswordByStaff;

public sealed class ResetUserPasswordByStaffValidator : AbstractValidator<ResetUserPasswordByStaffCommand>
{
    public ResetUserPasswordByStaffValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage($"{nameof(ResetUserPasswordByStaffCommand.Email)} is required.")
            .IsEmail();

        RuleFor(x => x.VerificationCode)
            .NotEmpty().WithMessage($"{nameof(ResetUserPasswordByStaffCommand.VerificationCode)} is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage($"{nameof(ResetUserPasswordByStaffCommand.NewPassword)} is required.")
            .IsPassword(nameof(ResetUserPasswordByStaffCommand.NewPassword));
    }
}
