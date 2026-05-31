using game_x.application.Extensions;

namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailForResetPassword;

public sealed class VerifyEmailForResetPasswordValidator : AbstractValidator<VerifyEmailForResetPasswordCommand>
{
    public VerifyEmailForResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage($"{nameof(VerifyEmailForResetPasswordCommand.Email)} must be not empty.")
            .IsEmail(nameof(VerifyEmailForResetPasswordCommand.Email));

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage($"{nameof(VerifyEmailForResetPasswordCommand.Code)} must be not empty.")
            .Length(8).WithMessage($"{nameof(VerifyEmailForResetPasswordCommand.Code)} must be 8 characters.");
    }
}
