using game_x.application.Extensions;

namespace game_x.application.Features.Auth.Client.Commands.UserLogin;

public sealed class UserLoginValidator : AbstractValidator<UserLoginCommand>
{
    public UserLoginValidator()
    {
        RuleFor(x => x.Email)
            .IsEmail();
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(UserLoginCommand.Password)} is required.");
    }
}
