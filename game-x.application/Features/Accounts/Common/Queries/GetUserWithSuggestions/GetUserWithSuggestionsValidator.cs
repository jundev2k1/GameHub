namespace game_x.application.Features.Accounts.Common.Queries.GetUserWithSuggestions;

public sealed class GetUserWithSuggestionsValidator : AbstractValidator<GetUserWithSuggestionsQuery>
{
    public GetUserWithSuggestionsValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(20).WithMessage("Page size must be less than or equal 20");

        RuleFor(x => x.Roles)
            .Must((roles) => (roles != null) && roles
                .All(role => role
                    is AppRoles.Admin
                    or AppRoles.Cs
                    or AppRoles.Talent
                    or AppRoles.User
                    or AppRoles.Guest))
            .When(roles => roles is not null);
    }
}
