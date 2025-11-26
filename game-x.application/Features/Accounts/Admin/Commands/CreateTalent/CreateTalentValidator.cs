using game_x.application.Extensions;
using game_x.application.Features.Auth.Client.Commands.RegisterUser;

namespace game_x.application.Features.Accounts.Admin.Commands.CreateTalent;

public sealed class CreateTalentValidator : AbstractValidator<CreateTalentCommand>
{
    public CreateTalentValidator()
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
