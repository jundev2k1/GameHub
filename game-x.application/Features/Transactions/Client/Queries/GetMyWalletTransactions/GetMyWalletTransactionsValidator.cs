using game_x.application.Common.Filters;
using game_x.application.Features.Games.Dtos;
using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Client.Queries.GetMyWalletTransactions;

public sealed class GetMyWalletTransactionsValidator : AbstractValidator<GetMyWalletTransactionsQuery>
{
    private readonly string[] _allowFilterFields =
    [
        "type",
        nameof(WalletTransactionDto.Id),
        nameof(WalletTransactionDto.UserId),
        nameof(WalletTransactionDto.GamePlatformId),
        nameof(WalletTransactionDto.Amount),
        nameof(WalletTransactionDto.UpdatedAt),
        nameof(WalletTransactionDto.CreatedAt)
    ];

    private readonly string[] _allowSortFields =
    [
        nameof(WalletTransactionDto.Status),
        nameof(WalletTransactionDto.Amount),
        nameof(WalletTransactionDto.Type),
        nameof(WalletTransactionDto.UpdatedAt),
        nameof(WalletTransactionDto.CreatedAt)
    ];

    public GetMyWalletTransactionsValidator()
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

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetMyWalletTransactionsQuery> context)
    {
        if (_allowFilterFields.All(f => !f.Equals(filter.Field, StringComparison.CurrentCultureIgnoreCase)))
            context.AddFailure($"Filter field {filter.Field} is not allowed.");

        if (filter.Field.Equals(nameof(WalletTransactionDto.Status), StringComparison.OrdinalIgnoreCase))
        {
            ValidateStatusField(filter.Value, context);
        }

        if (filter.Field.Equals(nameof(WalletTransactionDto.Type), StringComparison.OrdinalIgnoreCase))
        {
            ValidateTypeField(filter.Value, context);
        }
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetMyWalletTransactionsQuery> context)
    {
        if (_allowSortFields.All(f => !f.Equals(sort.Field, StringComparison.CurrentCultureIgnoreCase)))
            context.AddFailure($"Sort field '{sort.Field} is not allowed.");
    }

    private static void ValidateStatusField(string value, ValidationContext<GetMyWalletTransactionsQuery> context)
    {
        var arr = value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim().ToUpperInvariant())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        var allValidStatuses = Enum.GetNames<ChainTransactionStatus>().ToList();

        foreach (var val in arr)
        {
            var isValid = allValidStatuses.Any(x => x.Equals(val, StringComparison.InvariantCultureIgnoreCase));
            if (isValid) continue;
            context.AddFailure($"Invalid status value: {value}.");
        }
    }

    private static void ValidateTypeField(string value, ValidationContext<GetMyWalletTransactionsQuery> context)
    {
        var upperValue = value.Trim().ToUpperInvariant();
        var allValidTypes = Enum.GetNames<ChainTransactionType>().ToList();
        var isValid = allValidTypes.Any(x => x.Equals(upperValue, StringComparison.InvariantCultureIgnoreCase));
        if (!isValid)
            context.AddFailure($"Invalid status value: {value}.");
    }
}
