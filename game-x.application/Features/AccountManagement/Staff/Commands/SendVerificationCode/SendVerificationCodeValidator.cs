using game_x.application.Extensions;

namespace game_x.application.Features.AccountManagement.Staff.Commands.SendVerificationCode;

public sealed class SendVerificationCodeValidator : AbstractValidator<SendVerificationCodeCommand>
{
    public SendVerificationCodeValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage($"{nameof(SendVerificationCodeCommand.Email)} is required.")
            .IsEmail(nameof(SendVerificationCodeCommand.Email));
    }
}
