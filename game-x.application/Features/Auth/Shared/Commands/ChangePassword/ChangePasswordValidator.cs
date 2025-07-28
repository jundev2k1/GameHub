using game_x.application.Extensions;

namespace game_x.application.Features.Auth.Shared.Commands.ChangePassword;

public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(ChangePasswordCommand.Password)} is required.");
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage($"{nameof(ChangePasswordCommand.NewPassword)} is required.")
            .IsPassword(nameof(ChangePasswordCommand.NewPassword));
        RuleFor(x => x)
            .Must(x => x.Password != x.NewPassword)
            .WithMessage("New password must be different from the current password.");
    }
}
