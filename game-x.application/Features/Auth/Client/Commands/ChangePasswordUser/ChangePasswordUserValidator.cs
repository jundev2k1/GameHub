using game_x.application.Extensions;

namespace game_x.application.Features.Auth.Client.Commands.ChangePasswordUser;

public sealed class ChangePasswordUserValidator : AbstractValidator<ChangePasswordUserCommand>
{
    public ChangePasswordUserValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage($"{nameof(ChangePasswordUserCommand.Token)} must be not empty.");

        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage($"{nameof(ChangePasswordUserCommand.OldPassword)} must be not empty.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage($"{nameof(ChangePasswordUserCommand.NewPassword)} must be not empty.")
            .IsPassword(nameof(ChangePasswordUserCommand.NewPassword));
    }
}
