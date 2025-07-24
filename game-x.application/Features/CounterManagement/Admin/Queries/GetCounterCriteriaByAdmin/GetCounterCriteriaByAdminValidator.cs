using game_x.application.Common.Filters;

namespace game_x.application.Features.CounterManagement.Admin.Queries.GetCounterCriteriaByAdmin;

public sealed class GetCounterCriteriaByAdminValidator : AbstractValidator<GetCounterCriteriaByAdminQuery>
{
    private readonly string[] _allowFields =
    {
        nameof(Counter.CounterNumber),
        nameof(Counter.CounterName),
        nameof(Counter.Status),
        nameof(Counter.Location),
        nameof(Counter.Description),
        nameof(Counter.UpdatedAt),
        nameof(Counter.CreatedAt),
        "search"
    };

    public GetCounterCriteriaByAdminValidator()
    {
        RuleForEach(x => x.Filters)
            .Custom(ValidateFilterField);

        RuleForEach(x => x.Sorts)
            .Custom(ValidateSortField);

        RuleFor(x => x.PageIndex)
            .GreaterThan(0).WithMessage("Page index must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("Page size must be greater than or equal 1")
            .LessThanOrEqualTo(1000).WithMessage("Page size must be less than or equal 1000");
    }

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetCounterCriteriaByAdminQuery> context)
    {
        if (_allowFields.All(f => f.ToLower() != filter.Field.ToLower()))
            context.AddFailure($"Filter field '{filter.Field}' is not allowed.");
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetCounterCriteriaByAdminQuery> context)
    {
        if (_allowFields.All(f => f.ToLower() != sort.Field.ToLower()))
            context.AddFailure($"Sort field '{sort.Field}' is not allowed.");
    }
}