namespace game_x.application.Features.Games.Client.Commands.Etl998.ModifyBettingLimit;

public sealed class ModifyBettingLimitValidator : AbstractValidator<ModifyBettingLimitCommand>
{
    public ModifyBettingLimitValidator()
    {
        RuleFor(x => x.LimitId)
            .NotEmpty()
            .WithMessage($"{nameof(ModifyBettingLimitCommand.LimitId)} must not be empty.")
            .Must(BeAValidLimitId)
            .WithMessage("Invalid betting limit group.");
    }

    private static bool BeAValidLimitId(int limitId)
        => BettingLimitGroups.All.ContainsKey(limitId);
}