using game_x.application.Extensions;

namespace game_x.application.Features.AccountManagement.Staff.Commands.CheckExistedUserInfoByStaff;

public sealed class CheckExistedUserInfoByStaffValidator : AbstractValidator<CheckExistedUserInfoByStaffCommand>
{
    public CheckExistedUserInfoByStaffValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage($"{nameof(CheckExistedUserInfoByStaffCommand.Email)} is required.")
            .IsEmail();
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage($"{nameof(CheckExistedUserInfoByStaffCommand.PhoneNumber)} is required.")
            .IsPhoneNumber(nameof(CheckExistedUserInfoByStaffCommand.PhoneNumber));
    }
}
