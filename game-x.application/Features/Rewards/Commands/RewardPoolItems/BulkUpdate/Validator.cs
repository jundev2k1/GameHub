namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.BulkUpdate;

public sealed class BulkUpdateRewardPoolItemValidator
    : AbstractValidator<BulkUpdateRewardPoolItemCommand>
{
    public BulkUpdateRewardPoolItemValidator()
    {
        RuleFor(x => x.RewardPoolId)
            .NotEmpty();

        RuleFor(x => x.Items)
            .NotEmpty();

        RuleForEach(x => x.Items).ChildRules(item =>
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
    }
}