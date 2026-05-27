namespace game_x.application.Features.Games.Admin.Commands.UpsertGameMedias;

public sealed class UpsertGameMediasValidator : AbstractValidator<UpsertGameMediasCommand>
{
    public UpsertGameMediasValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage($"{nameof(UpsertGameMediasCommand.Id)} is required.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage($"{nameof(UpsertGameMediasCommand.Items)} must not be empty.")
            .Must(items => items == null || items.Select(i => i.Id).Where(id => id.HasValue).Distinct().Count() == items.Select(i => i.Id).Where(id => id.HasValue).Count())
            .WithMessage($"{nameof(UpsertGameMediasCommand.Items)} contains duplicate Media IDs.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Type)
                .IsInEnum().WithMessage("Game Media Type is invalid or out of range.");

            item.RuleFor(i => i.Category)
                .IsInEnum().WithMessage("Game Media Category is invalid or out of range.");

            item.RuleFor(i => i.Title)
                .NotEmpty().WithMessage($"{nameof(GameMediaItemInfo.Title)} is required.")
                .MaximumLength(256).WithMessage($"{nameof(GameMediaItemInfo.Title)} must not exceed 256 characters.");

            item.RuleFor(i => i.Note)
                .MaximumLength(4000).WithMessage($"{nameof(GameMediaItemInfo.Note)} must not exceed 4000 characters.");

            item.RuleFor(i => i.Priority)
                .GreaterThanOrEqualTo(0).WithMessage($"{nameof(GameMediaItemInfo.Priority)} must be greater than or equal to 0.");
        });
    }
}
