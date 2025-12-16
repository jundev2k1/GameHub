using game_x.application.Extensions;

namespace game_x.application.Features.Accounts.Admin.Commands.CreateTalent;

public sealed class CreateTalentValidator : AbstractValidator<CreateTalentCommand>
{
    public CreateTalentValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage($"{nameof(CreateTalentCommand.Username)} must be not empty.")
            .MinimumLength(6).WithMessage($"{nameof(CreateTalentCommand.Username)} must be less than 6 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage($"{nameof(CreateTalentCommand.Password)} must be not empty.")
            .IsPassword();

        RuleFor(x => x.Nickname)
            .NotEmpty().WithMessage($"{nameof(CreateTalentCommand.Nickname)} must be not empty.")
            .MaximumLength(20).WithMessage($"{nameof(CreateTalentCommand.Nickname)} must be not greater than 20 characters.");
    }
}
