using game_x.application.Extensions;

namespace game_x.application.Features.AccountManagement.Staff.Commands.SendResetUserPasswordCodeByStaff;

public sealed class SendResetUserPasswordCodeByStaffValidator : AbstractValidator<SendResetUserPasswordCodeByStaffCommand>
{
    public SendResetUserPasswordCodeByStaffValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage($"{nameof(SendResetUserPasswordCodeByStaffCommand.Email)} is required.")
            .IsEmail(nameof(SendResetUserPasswordCodeByStaffCommand.Email));
    }
}
