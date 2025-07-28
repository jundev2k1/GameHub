using game_x.application.Extensions;

namespace game_x.application.Features.Auth.Commands.Register.RegisterUser;

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage($"{nameof(RegisterUserCommand.Email)} must be not empty.")
            .IsEmail();

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(RegisterUserCommand.Password)} must be not empty.")
            .IsPassword();

        RuleFor(x => x.Nickname)
            .NotEmpty().WithMessage($"{nameof(RegisterUserCommand.Nickname)} must be not empty.")
            .MaximumLength(20).WithMessage($"{nameof(RegisterUserCommand.Nickname)} must be not greater than 20 characters.");
    }
}
