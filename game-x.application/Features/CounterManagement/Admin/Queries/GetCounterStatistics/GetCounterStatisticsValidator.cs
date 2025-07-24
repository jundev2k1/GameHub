using game_x.application.Common.Filters;

namespace game_x.application.Features.CounterManagement.Admin.Queries.GetCounterStatistics;

public sealed class GetCounterStatisticsValidator : AbstractValidator<GetCounterStatisticsQuery>
{
    private readonly string[] _allowFields =
    {
        nameof(Order.OrderStatus),
        nameof(Order.OrderType),
        nameof(Order.FiatType),
        nameof(Order.CryptoType),
        nameof(Order.UpdatedAt),
        nameof(Order.CreatedAt)
    };

    public GetCounterStatisticsValidator()
    {
        RuleForEach(x => x.Filters)
            .Custom(ValidateFilterField);
    }

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetCounterStatisticsQuery> context)
    {
        if (_allowFields.All(f => f.ToLower() != filter.Field.ToLower()))
            context.AddFailure($"Filter field '{filter.Field}' is not allowed.");
    }
}