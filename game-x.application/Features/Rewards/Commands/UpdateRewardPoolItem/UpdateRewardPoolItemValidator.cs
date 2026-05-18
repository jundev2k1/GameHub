namespace game_x.application.Features.Rewards.Commands.UpdateRewardPoolItem;

public sealed class UpdateRewardPoolItemValidator : AbstractValidator<UpdateRewardPoolItemCommand>
{
    public UpdateRewardPoolItemValidator()
    {
        RuleFor(x => x.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than zero.")
            .When(x => x.Weight is not null);
        
        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("SortOrder must be greater than or equal to zero.")
            .When(x => x.SortOrder is not null);
        
        // RuleFor(x => x.Config)
        //     .NotNull()
        //     .SetValidator(new RewardPoolConfigDataValidator());
    }
}