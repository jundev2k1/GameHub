using game_x.application.Common.Filters;

namespace game_x.application.Features.OrderManagement.Client.Queries.GetOrderCriteriaByClient;

public sealed class GetOrderCriteriaByClientValidator : AbstractValidator<GetOrderCriteriaByClientQuery>
{
    private readonly string[] _allowFields =
    {
        "search",
        nameof(Order.PublicId),
        nameof(Order.OrderType),
        nameof(Order.OrderStatus),
        nameof(Order.UserId),
        nameof(Order.CounterId),
        nameof(Order.Quantity),
        nameof(Order.StaffId),
        nameof(Order.PricePerUnit),
        nameof(Order.TotalPrice),
        nameof(Order.CurrencyUnit),
        nameof(Order.UpdatedAt),
        nameof(Order.CreatedAt)
    };

    public GetOrderCriteriaByClientValidator()
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

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetOrderCriteriaByClientQuery> context)
    {
        if (_allowFields.All(f => f.ToLower() != filter.Field.ToLower()))
            context.AddFailure($"Filter field '{filter.Field}' is not allowed.");
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetOrderCriteriaByClientQuery> context)
    {
        if (_allowFields.All(f => f.ToLower() != sort.Field.ToLower()))
            context.AddFailure($"Sort field '{sort.Field}' is not allowed.");
    }
}