namespace game_x.application.Features.Games.Admin.Commands.UpdateGameType;

public sealed class UpdateGameTypeValidator : AbstractValidator<UpdateGameTypeCommand>
{
    public UpdateGameTypeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(UpdateGameTypeCommand.Name)} is required.")
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateGameTypeCommand.Description)} must be less than 4000 chacracters.");

        RuleFor(x => x.Note)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateGameTypeCommand.Note)} must be less than 4000 chacracters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(UpdateGameTypeCommand.Priority)} must be greater than or equal 0.");
    }
}
