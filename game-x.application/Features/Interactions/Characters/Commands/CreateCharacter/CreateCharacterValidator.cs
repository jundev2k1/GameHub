namespace game_x.application.Features.Interactions.Characters.Commands.CreateCharacter;

public sealed class CreateCharacterValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(CreateCharacterCommand.Name)} is required.")
            .MaximumLength(255).WithMessage($"{nameof(CreateCharacterCommand.Name)} cannot exceed 255 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage($"{nameof(CreateCharacterCommand.Description)} cannot exceed 1000 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(4000).WithMessage($"{nameof(CreateCharacterCommand.Notes)} cannot exceed 4000 characters.");
    }
}
