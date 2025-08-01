using game_x.application.Extensions;

namespace game_x.application.Features.Auth.Client.Commands.ResetPassword;

public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage($"{nameof(ResetPasswordCommand.OldPassword)} must be not empty.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage($"{nameof(ResetPasswordCommand.NewPassword)} must be not empty.")
            .IsPassword(nameof(ResetPasswordCommand.NewPassword));
    }
}
