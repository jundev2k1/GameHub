namespace game_x.application.Features.Interactions.Characters.Commands.UpdateCharacter;

public sealed class UpdateCharacterValidator : AbstractValidator<UpdateCharacterCommand>
{
    public UpdateCharacterValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.Notes)
            .MaximumLength(4000).WithMessage("Notes cannot exceed 4000 characters.")
            .When(x => x.Notes is not null);
    }
}
