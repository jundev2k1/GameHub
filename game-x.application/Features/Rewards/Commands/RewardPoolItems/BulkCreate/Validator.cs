namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.BulkCreate;

public sealed class BulkCreateRewardPoolItemValidator
    : AbstractValidator<BulkCreateRewardPoolItemCommand>
{
    public BulkCreateRewardPoolItemValidator()
    {
        RuleFor(x => x.RewardPoolId)
            .NotEmpty();

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required.");

        RuleForEach(x => x.Items)
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
    }
}