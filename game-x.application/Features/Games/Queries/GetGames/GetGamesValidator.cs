namespace game_x.application.Features.Games.Queries.GetGames;

public sealed class GetGamesValidator : AbstractValidator<GetGamesQuery>
{
    public GetGamesValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0).WithMessage("Page index must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be greater than or equal 1")
            .LessThanOrEqualTo(1000).WithMessage("Page size must be less than or equal 1000");
    }
}
