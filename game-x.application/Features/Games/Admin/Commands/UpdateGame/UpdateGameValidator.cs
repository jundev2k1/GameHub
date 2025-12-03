namespace game_x.application.Features.Games.Admin.Commands.UpdateGame;

public sealed class UpdateGameValidator : AbstractValidator<UpdateGameCommand>
{
    public UpdateGameValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage($"{nameof(UpdateGameCommand.Id)} is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage($"{nameof(UpdateGameCommand.Name)} is required.")
            .MaximumLength(256).WithMessage($"{nameof(UpdateGameCommand.Name)} must not exceed 256 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateGameCommand.Description)} must not exceed 4000 characters.");

        RuleFor(x => x.Note)
            .MaximumLength(4000).WithMessage($"{nameof(UpdateGameCommand.Note)} must not exceed 4000 characters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage($"{nameof(UpdateGameCommand.Priority)} must be greater than or equal to 0.");

        RuleFor(x => x.Categories)
            .Must(items => items!.Length == 0 || items.Count(i => i.IsPrimary) == 1)
                .WithMessage($"There must be exactly one primary Categories.")
            .Must(items => items!.Select(i => i.Id).Distinct().Count() == items!.Length)
                .WithMessage($"Categories contains duplicate IDs.");

        RuleFor(x => x.Types)
            .Must(items => items!.Length == 0 || items.Count(i => i.IsPrimary) == 1)
                .WithMessage($"There must be exactly one primary Types.")
            .Must(items => items!.Select(i => i.Id).Distinct().Count() == items!.Length)
                .WithMessage("Types contains duplicate IDs.");

        RuleFor(x => x.Tags)
            .Must(items => items!.Length == 0 || items.Count(i => i.IsPrimary) == 1)
                .WithMessage($"There must be exactly one primary Tags.")
            .Must(items => items!.Select(i => i.Id).Distinct().Count() == items!.Length)
                .WithMessage($"Tags contains duplicate IDs."); ;
    }

    private static IRuleBuilderOptions<T, TItem[]> ValidateCollection<T, TItem>(
        IRuleBuilder<T, TItem[]> rule,
        string fieldName)
    {
        return rule
            .Must(items => items!.Length == 0 || items.Count(i => i.IsPrimary) == 1)
                .WithMessage($"There must be exactly one primary {fieldName}.")
            .Must(items => items!.Select(i => i.Id).Distinct().Count() == items!.Length)
                .WithMessage($"{fieldName} contains duplicate IDs.");
    }
}
