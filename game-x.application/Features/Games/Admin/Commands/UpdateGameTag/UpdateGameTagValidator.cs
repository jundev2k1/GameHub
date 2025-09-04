namespace game_x.application.Features.Games.Admin.Commands.UpdateGameTag;

public sealed class UpdateGameTagValidator : AbstractValidator<UpdateGameTagCommand>
{
    public UpdateGameTagValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(UpdateGameTagCommand.Name)} is required.")
            .MaximumLength(50);

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage($"{nameof(UpdateGameTagCommand.Color)} is required.")
            .Must(GameTagColor.IsValid).WithMessage($"{nameof(UpdateGameTagCommand.Color)} is invalid.");

        RuleFor(x => x.Icon)
            .Must(GameTagIcon.IsValid).WithMessage($"{nameof(UpdateGameTagCommand.Icon)} is invalid.");

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateGameTagCommand.Description)} must be less than 4000 chacracters.");

        RuleFor(x => x.Note)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateGameTagCommand.Note)} must be less than 4000 chacracters.");
    }
}
