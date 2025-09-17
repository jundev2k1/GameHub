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
            .Must(items => items!.Length > 0 && items.Count(item => item.IsPrimary) == 1)
                .WithMessage("There must be exactly one primary category.")
            .Must(items => items!.Select(i => i.Id).Distinct().Count() == items!.Length)
                .WithMessage($"{nameof(UpdateGameCommand.Categories)} contains duplicate category IDs.")
            .When(x => x.Categories is not null);

        RuleFor(x => x.Types)
            .Must(items => items!.Length > 0 && items.Count(item => item.IsPrimary) == 1)
                .WithMessage("There must be exactly one primary type.")
            .Must(items => items!.Select(i => i.Id).Distinct().Count() == items!.Length)
                .WithMessage($"{nameof(UpdateGameCommand.Categories)} contains duplicate type IDs.")
            .When(x => x.Types is not null);

        RuleFor(x => x.Tags)
            .Must(items => items!.Length > 0 && items.Count(item => item.IsPrimary) == 1)
                .WithMessage("There must be exactly one primary tag.")
            .Must(items => items!.Select(i => i.Id).Distinct().Count() == items!.Length)
                .WithMessage($"{nameof(UpdateGameCommand.Categories)} contains duplicate tag IDs.")
            .When(x => x.Tags is not null);
    }
}
