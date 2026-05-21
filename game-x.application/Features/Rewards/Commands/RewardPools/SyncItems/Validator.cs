namespace game_x.application.Features.Rewards.Commands.RewardPools.SyncItems;

public sealed class SyncRewardPoolItemCommandValidator
    : AbstractValidator<SyncRewardPoolItemCommand>
{
    public SyncRewardPoolItemCommandValidator()
    {
        RuleFor(x => x.RewardPoolId)
            .NotEmpty();

        RuleForEach(x => x.CreatedItems)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.RewardDefinitionId)
                    .NotEmpty();

                item.RuleFor(x => x.Weight)
                    .GreaterThan(0);

                item.RuleFor(x => x.SortOrder)
                    .GreaterThanOrEqualTo(0);

                item.RuleFor(x => x.EndAt)
                    .GreaterThan(x => x.StartAt!.Value)
                    .When(x => x.StartAt.HasValue && x.EndAt.HasValue);
            });

        RuleForEach(x => x.UpdatedItems)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.Id)
                    .NotEmpty();

                item.RuleFor(x => x.Weight)
                    .GreaterThan(0)
                    .When(x => x.Weight.HasValue);

                item.RuleFor(x => x.SortOrder)
                    .GreaterThanOrEqualTo(0)
                    .When(x => x.SortOrder.HasValue);

                item.RuleFor(x => x.EndAt)
                    .GreaterThan(x => x.StartAt!.Value)
                    .When(x => x.StartAt.HasValue && x.EndAt.HasValue);
            });

        RuleFor(x => x.DeletedItems)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate deleted item ids are not allowed.");

        RuleFor(x => x.UpdatedItems)
            .Must(items => items.Select(i => i.Id).Distinct().Count() == items.Count)
            .WithMessage("Duplicate updated item ids are not allowed.");

        RuleFor(x => x)
            .Must(NoUpdateDeleteConflict)
            .WithMessage("An item cannot be updated and deleted at the same time.");
    }

    private static bool NoUpdateDeleteConflict(SyncRewardPoolItemCommand cmd)
    {
        var updatedIds = cmd.UpdatedItems
            .Select(x => x.Id)
            .ToHashSet();

        return !cmd.DeletedItems.Any(updatedIds.Contains);
    }
}