using game_x.application.Common.Filters;

namespace game_x.application.Features.BankAccountManagement.Client.Queries.GetBankAccountCriteriaByUser;

public sealed class GetBankAccountCriteriaByUserValidator : AbstractValidator<GetBankAccountByCriteriaQuery>
{
    private readonly string[] _allowFields =
    {
        nameof(BankAccount.BankAccountName),
        "search"
    };

    public GetBankAccountCriteriaByUserValidator()
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

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetBankAccountByCriteriaQuery> context)
    {
        if (!_allowFields.Contains(filter.Field))
            context.AddFailure($"Filter field '{filter.Field}' is not allowed.");
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetBankAccountByCriteriaQuery> context)
    {
        if (!_allowFields.Contains(sort.Field))
            context.AddFailure($"Sort field '{sort.Field}' is not allowed.");
    }
}
