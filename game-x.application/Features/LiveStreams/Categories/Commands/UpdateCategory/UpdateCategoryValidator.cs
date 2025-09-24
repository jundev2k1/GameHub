namespace game_x.application.Features.LiveStreams.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(UpdateCategoryCommand.Name)} is required.")
            .MaximumLength(255).WithMessage($"{nameof(UpdateCategoryCommand.Name)} cannot exceed 255 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage($"{nameof(UpdateCategoryCommand.Description)} is required.")
            .MaximumLength(4000).WithMessage($"{nameof(UpdateCategoryCommand.Description)} cannot exceed 4000 characters.");

        RuleFor(x => x.Note)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateCategoryCommand.Note)} cannot exceed 4000 characters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(UpdateCategoryCommand.Priority)} must be greater than or equal 0.");
    }
}
