using game_x.application.Common.Filters;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Queries.GetMyTransactions;

public sealed class GetMyTransactionsValidator : AbstractValidator<GetMyTransactionsQuery>
{
    private readonly string[] _allowFilterFields =
    [
        "search",
        nameof(ChainTransactionDto.Id),
        nameof(ChainTransactionDto.UserId),
        nameof(ChainTransactionDto.Status),
        nameof(ChainTransactionDto.Amount),
        nameof(ChainTransactionDto.Type),
        nameof(ChainTransactionDto.UpdatedAt),
        nameof(ChainTransactionDto.CreatedAt)
    ];
    
    private readonly string[] _allowSortFields =
    [
        nameof(ChainTransactionDto.Status),
        nameof(ChainTransactionDto.Amount),
        nameof(ChainTransactionDto.Type),
        nameof(ChainTransactionDto.UpdatedAt),
        nameof(ChainTransactionDto.CreatedAt)
    ];
    
    public GetMyTransactionsValidator()
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

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetMyTransactionsQuery> context)
    {
        if (_allowFilterFields.All(f => f.ToLower() != filter.Field.ToLower()))
            context.AddFailure($"Filter field {filter.Field} is not allowed.");
            
        if (filter.Field.Equals(nameof(ChainTransactionDto.Status), StringComparison.OrdinalIgnoreCase))
        {
            ValidateStatusField(filter.Value, context);
        }
        
        if (filter.Field.Equals(nameof(ChainTransactionDto.Type), StringComparison.OrdinalIgnoreCase))
        {
            ValidateTypeField(filter.Value, context);
        }
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetMyTransactionsQuery> context)
    {
        if (_allowSortFields.All(f => f.ToLower() != sort.Field.ToLower()))
            context.AddFailure($"Sort field '{sort.Field} is not allowed.");
    }
    
    private void ValidateStatusField(string value, ValidationContext<GetMyTransactionsQuery> context)
    {
        var arr = value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim().ToUpperInvariant())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
        
        var allValidStatuses = Enum.GetNames(typeof(ChainTransactionStatus)).ToList();
        
        foreach (var val in arr)
        {
            var isValid = allValidStatuses.Any(x => x.ToUpperInvariant() == val);
            if (isValid) continue;
            context.AddFailure($"Invalid status value: {value}.");
        }
    }
    
    private void ValidateTypeField(string value, ValidationContext<GetMyTransactionsQuery> context)
    {
        var upperValue = value.Trim().ToUpperInvariant();
        var allValidTypes = Enum.GetNames(typeof(ChainTransactionType)).ToList();
        var isValid = allValidTypes.Any(x => x.ToUpperInvariant() == upperValue);
        if (!isValid)
            context.AddFailure($"Invalid status value: {value}.");
    }
}