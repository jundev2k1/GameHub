namespace game_x.application.Features.Rewards.Queries.Missions.GetListForUser;

public sealed class GetMissionListForUserValidator : AbstractValidator<GetMissionListForUserQuery>
{
    public GetMissionListForUserValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Type is invalid.")
            .When(x => x.Type is not null);
    }
}