namespace game_x.application.Features.AccountManagement.Staff.Commands.ValidateUserQrCode;

public sealed class ValidateUserQrCodeValidator : AbstractValidator<ValidateUserQrCodeCommand>
{
    public ValidateUserQrCodeValidator()
    {
        RuleFor(x => x.QrCode)
            .NotEmpty().WithMessage($"{nameof(ValidateUserQrCodeCommand.QrCode)} is required.");
    }
}
