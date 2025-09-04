namespace game_x.application.Features.Games.Admin.Queries.GetGameTagsByCriteria;

public sealed class GetGameTagsByCriteriaValidator : AbstractValidator<GetGameTagsByCriteriaQuery>
{
    public GetGameTagsByCriteriaValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0).WithMessage("Page index must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be less than or equal to 100.");
    }
}
