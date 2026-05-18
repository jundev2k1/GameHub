namespace game_x.application.Features.Rewards.Commands.CreateRewardPoolItem;

public sealed class CreateRewardPoolItemValidator : AbstractValidator<CreateRewardPoolItemCommand>
{
    public CreateRewardPoolItemValidator()
    {
        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("SortOrder must be greater than or equal to zero.");
        
        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Weight must be greater than or equal to zero.");
    }
}