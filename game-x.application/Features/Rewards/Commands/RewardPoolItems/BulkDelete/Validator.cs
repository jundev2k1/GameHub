namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.BulkDelete;

public sealed class BulkDeleteRewardPoolItemValidator : AbstractValidator<BulkDeleteRewardPoolItemCommand>
{
    public BulkDeleteRewardPoolItemValidator()
    {
        RuleFor(x => x.RewardPoolId)
            .NotEmpty()
            .WithMessage("RewardPoolId is required.");

        RuleFor(x => x.ItemIds)
            .NotEmpty()
            .WithMessage("At least one item must be selected.");

        RuleFor(x => x.ItemIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate item ids are not allowed.");
    }
}