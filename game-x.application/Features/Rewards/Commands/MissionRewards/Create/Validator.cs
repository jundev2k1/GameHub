namespace game_x.application.Features.Rewards.Commands.MissionRewards.Create;

public sealed class CreateMissionRewardValidator : AbstractValidator<CreateMissionRewardCommand>
{
    public CreateMissionRewardValidator()
    {
        RuleFor(x => x.Sequence)
            .GreaterThan(0)
            .WithMessage("Sequence must be greater than zero");
        
        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("SortOrder must be greater than or equal to or equal to 0");
        
        RuleFor(x => x.RequiredProgress)
            .GreaterThan(0)
            .WithMessage("RequiredProgress must be greater than zero");
    }
}