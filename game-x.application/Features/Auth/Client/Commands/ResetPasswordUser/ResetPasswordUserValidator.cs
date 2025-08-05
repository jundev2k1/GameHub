using game_x.application.Extensions;

namespace game_x.application.Features.Auth.Client.Commands.ResetPasswordUser;

public sealed class ResetPasswordUserValidator : AbstractValidator<ResetPasswordUserCommand>
{
    public ResetPasswordUserValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage($"{nameof(ResetPasswordUserCommand.Token)} must be not empty.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(ResetPasswordUserCommand.Password)} must be not empty.")
            .IsPassword(nameof(ResetPasswordUserCommand.Password));
    }
}
