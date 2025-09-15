namespace game_x.application.Features.LiveStreams.Commands.CreateCategory;

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(CreateCategoryCommand.Name)} is required.")
            .MaximumLength(255).WithMessage($"{nameof(CreateCategoryCommand.Name)} cannot exceed 255 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage($"{nameof(CreateCategoryCommand.Description)} is required.")
            .MaximumLength(4000).WithMessage($"{nameof(CreateCategoryCommand.Description)} cannot exceed 4000 characters.");

        RuleFor(x => x.Note)
            .MaximumLength(4000).WithMessage($"{nameof(CreateCategoryCommand.Note)} cannot exceed 4000 characters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(CreateCategoryCommand.Priority)} must be greater than or equal 0.");
    }
}
