using game_x.application.Extensions;

namespace game_x.application.Features.Auth.Commands.Register.Client;

public sealed class RegistUserValidator : AbstractValidator<RegistUserCommand>
{
    public RegistUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage($"{nameof(RegistUserCommand.Email)} must be not empty.")
            .IsEmail();

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(RegistUserCommand.Password)} must be not empty.")
            .IsPassword();

        RuleFor(x => x.Nickname)
            .NotEmpty().WithMessage($"{nameof(RegistUserCommand.Nickname)} must be not empty.")
            .MaximumLength(20).WithMessage($"{nameof(RegistUserCommand.Nickname)} must be not greater than 20 characters.");
    }
}
