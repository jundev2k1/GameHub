using game_x.application.Common.Filters;
using game_x.application.Features.TalentWallets.DTOs;

namespace game_x.application.Features.TalentWallets.Queries.GetMyTalentWalletTransactions;

public sealed class GetMyTalentWalletTransactionsValidator : AbstractValidator<GetMyTalentWalletTransactionsQuery>
{
    private readonly string[] _allowFields =
    {
        nameof(TalentWalletTransactionDto.Id),
        nameof(TalentWalletTransactionDto.Type),
        nameof(TalentWalletTransactionDto.CreatedAt),
    };

    public GetMyTalentWalletTransactionsValidator()
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

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetMyTalentWalletTransactionsQuery> context)
    {
        if (_allowFields.All(f => !f.Equals(filter.Field, StringComparison.InvariantCultureIgnoreCase)))
            context.AddFailure($"Filter field '{filter.Field}' is not allowed.");
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetMyTalentWalletTransactionsQuery> context)
    {
        if (_allowFields.All(f => !f.Equals(sort.Field, StringComparison.InvariantCultureIgnoreCase)))
            context.AddFailure($"Sort field '{sort.Field}' is not allowed.");
    }
}
