namespace game_x.application.Features.Games.Admin.Commands.CreateGameTag;

public sealed class CreateGameTagValidator : AbstractValidator<CreateGameTagCommand>
{
    public CreateGameTagValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(CreateGameTagCommand.Name)} is required.")
            .MaximumLength(50);

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage($"{nameof(CreateGameTagCommand.Color)} is required.")
            .Must(GameTagColor.IsValid).WithMessage($"{nameof(CreateGameTagCommand.Color)} is invalid.");

        RuleFor(x => x.Icon)
            .Must(GameTagIcon.IsValid).WithMessage($"{nameof(CreateGameTagCommand.Icon)} is invalid.");

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage($"{nameof(CreateGameTagCommand.Description)} must be less than 4000 chacracters.");

        RuleFor(x => x.Note)
            .MaximumLength(4000).WithMessage($"{nameof(CreateGameTagCommand.Note)} must be less than 4000 chacracters.");
    }
}
