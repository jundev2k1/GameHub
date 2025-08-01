using game_x.application.Extensions;

namespace game_x.application.Features.Auth.Admin.Commands.ChangePasswordAdmin;

public sealed class ChangePasswordAdminValidator : AbstractValidator<ChangePasswordAdminCommand>
{
    public ChangePasswordAdminValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(ChangePasswordAdminCommand.Password)} is required.");
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage($"{nameof(ChangePasswordAdminCommand.NewPassword)} is required.")
            .IsPassword(nameof(ChangePasswordAdminCommand.NewPassword));
        RuleFor(x => x)
            .Must(x => x.Password != x.NewPassword)
            .WithMessage("New password must be different from the current password.");
    }
}
