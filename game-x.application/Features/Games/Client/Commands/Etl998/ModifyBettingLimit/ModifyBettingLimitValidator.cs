namespace game_x.application.Features.Games.Client.Commands.Etl998.ModifyBettingLimit;

public sealed class ModifyBettingLimitValidator : AbstractValidator<ModifyBettingLimitCommand>
{
    public ModifyBettingLimitValidator()
    {
        RuleFor(x => x.Tables)
            .NotEmpty().WithMessage($"{nameof(ModifyBettingLimitCommand.Tables)} must be not empty.");
    }
}