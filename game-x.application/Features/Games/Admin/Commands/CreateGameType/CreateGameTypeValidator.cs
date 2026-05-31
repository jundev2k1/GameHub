namespace game_x.application.Features.Games.Admin.Commands.CreateGameType;

public sealed class CreateGameTypeValidator : AbstractValidator<CreateGameTypeCommand>
{
    public CreateGameTypeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(CreateGameTypeCommand.Name)} is required.")
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage($"{nameof(CreateGameTypeCommand.Description)} must be less than 4000 chacracters.");

        RuleFor(x => x.Note)
            .MaximumLength(4000).WithMessage($"{nameof(CreateGameTypeCommand.Note)} must be less than 4000 chacracters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(CreateGameTypeCommand.Priority)} must be greater than or equal 0.");
    }
}
