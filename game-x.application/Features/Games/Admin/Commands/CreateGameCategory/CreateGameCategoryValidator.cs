namespace game_x.application.Features.Games.Admin.Commands.CreateGameCategory;

public sealed class CreateGameCategoryValidator : AbstractValidator<CreateGameCategoryCommand>
{
    public CreateGameCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(CreateGameCategoryCommand.Name)} is required.")
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage($"{nameof(CreateGameCategoryCommand.Description)} must be less than 4000 chacracters.");

        RuleFor(x => x.Note)
            .MaximumLength(4000).WithMessage($"{nameof(CreateGameCategoryCommand.Note)} must be less than 4000 chacracters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(CreateGameCategoryCommand.Priority)} must be greater than or equal 0.");
    }
}
