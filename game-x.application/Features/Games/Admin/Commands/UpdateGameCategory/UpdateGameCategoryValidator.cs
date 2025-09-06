namespace game_x.application.Features.Games.Admin.Commands.UpdateGameCategory;

public sealed class UpdateGameCategoryValidator : AbstractValidator<UpdateGameCategoryCommand>
{
    public UpdateGameCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(UpdateGameCategoryCommand.Name)} is required.")
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateGameCategoryCommand.Description)} must be less than 4000 chacracters.");

        RuleFor(x => x.Note)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateGameCategoryCommand.Note)} must be less than 4000 chacracters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(UpdateGameCategoryCommand.Priority)} must be greater than or equal 0.");
    }
}
