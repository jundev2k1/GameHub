namespace game_x.application.Features.Rewards.Commands.Missions.SyncItems;

public sealed class SyncMissionRewardCommandValidator
    : AbstractValidator<SyncMissionRewardCommand>
{
    public SyncMissionRewardCommandValidator()
    {
        RuleFor(x => x.MissionId)
            .NotEmpty();

        RuleForEach(x => x.CreatedItems)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.RewardDefinitionId)
                    .NotEmpty();

                item.RuleFor(x => x.Sequence)
                    .GreaterThan(0);

                item.RuleFor(x => x.SortOrder)
                    .GreaterThanOrEqualTo(0);

                item.RuleFor(x => x.RequiredProgress)
                    .GreaterThan(0);

                item.RuleFor(x => x.EndAt)
                    .GreaterThan(x => x.StartAt!.Value)
                    .When(x => x.StartAt.HasValue && x.EndAt.HasValue);
            });

        RuleForEach(x => x.UpdatedItems)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.Id)
                    .NotEmpty();

                item.RuleFor(x => x.RewardDefinitionId)
                    .NotEmpty();

                item.RuleFor(x => x.Sequence)
                    .GreaterThan(0);

                item.RuleFor(x => x.SortOrder)
                    .GreaterThanOrEqualTo(0);

                item.RuleFor(x => x.RequiredProgress)
                    .GreaterThan(0);

                item.RuleFor(x => x.EndAt)
                    .GreaterThan(x => x.StartAt!.Value)
                    .When(x => x.StartAt.HasValue && x.EndAt.HasValue);
            });

        RuleFor(x => x.DeletedItems)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate deleted ids are not allowed.");

        RuleFor(x => x.UpdatedItems)
            .Must(items => items.Select(i => i.Id).Distinct().Count() == items.Count)
            .WithMessage("Duplicate updated ids are not allowed.");

        RuleFor(x => x)
            .Must(NoDuplicateSequences)
            .WithMessage("Duplicate sequence values are not allowed.");

        RuleFor(x => x)
            .Must(NoUpdateDeleteConflict)
            .WithMessage("An item cannot be updated and deleted at the same time.");
    }

    private static bool NoDuplicateSequences(SyncMissionRewardCommand cmd)
    {
        var sequences = cmd.CreatedItems
            .Select(x => x.Sequence)
            .Concat(cmd.UpdatedItems.Select(x => x.Sequence))
            .ToList();

        return sequences.Distinct().Count() == sequences.Count();
    }

    private static bool NoUpdateDeleteConflict(SyncMissionRewardCommand cmd)
    {
        var updatedIds = cmd.UpdatedItems
            .Select(x => x.Id)
            .ToHashSet();

        return !cmd.DeletedItems.Any(updatedIds.Contains);
    }
}