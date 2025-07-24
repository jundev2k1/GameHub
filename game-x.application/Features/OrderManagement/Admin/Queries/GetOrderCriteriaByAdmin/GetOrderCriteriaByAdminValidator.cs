using game_x.application.Common.Filters;
using game_x.application.Extensions.FilterExtensions;

namespace game_x.application.Features.OrderManagement.Admin.Queries.GetOrderCriteriaByAdmin;

public sealed class GetOrderCriteriaByAdminValidator : AbstractValidator<GetOrderCriteriaByAdminQuery>
{
    private readonly string[] _allowFields =
    {
        "search",
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

    public GetOrderCriteriaByAdminValidator()
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

    private void ValidateFilterField(QueryFilter filter, ValidationContext<GetOrderCriteriaByAdminQuery> context)
    {
        if (_allowFields.All(f => f.ToLower() != filter.Field.ToLower()))
            context.AddFailure($"Filter field '{filter.Field}' is not allowed.");
            
        if (filter.Field.Equals(nameof(Order.OrderStatus), StringComparison.OrdinalIgnoreCase))
        {
            ValidateCustomField(filter.Value?.ToString() ?? "", context);
        }
    }

    private void ValidateSortField(QuerySort sort, ValidationContext<GetOrderCriteriaByAdminQuery> context)
    {
        if (_allowFields.All(f => f.ToLower() != sort.Field.ToLower()))
            context.AddFailure($"Sort field '{sort.Field}' is not allowed.");
    }


    private void ValidateCustomField(string value, ValidationContext<GetOrderCriteriaByAdminQuery> context)
    {
        var arr = value.Split(",", StringSplitOptions.RemoveEmptyEntries);

        foreach (var val in arr)
        {

            var IsValid = OrderStatus.IsValid(val);

            if (IsValid) continue;

            context.AddFailure($"Invalid order status value: '{val}'.");

        }

    }
}
