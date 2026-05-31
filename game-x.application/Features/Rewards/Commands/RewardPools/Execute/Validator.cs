namespace game_x.application.Features.Rewards.Commands.RewardPools.Execute;

public sealed class RewardPoolExecuteCommandValidator : AbstractValidator<RewardPoolExecuteCommand>
{
    public RewardPoolExecuteCommandValidator()
    {
        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
            .WithMessage("IdempotencyKey is required.");
    }
}