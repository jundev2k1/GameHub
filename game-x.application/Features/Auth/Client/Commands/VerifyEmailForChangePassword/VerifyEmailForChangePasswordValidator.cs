namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailForChangePassword;

public sealed class VerifyEmailForChangePasswordValidator : AbstractValidator<VerifyEmailForChangePasswordCommand>
{
    public VerifyEmailForChangePasswordValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage($"{nameof(VerifyEmailForChangePasswordCommand.Code)} must be not empty.")
            .Length(8).WithMessage($"{nameof(VerifyEmailForChangePasswordCommand.Code)} must be 8 characters.");
    }
}
