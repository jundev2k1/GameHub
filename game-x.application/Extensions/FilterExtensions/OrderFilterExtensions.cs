using System.Linq.Expressions;
using Microsoft.VisualBasic;

namespace game_x.application.Extensions.FilterExtensions;

public static class OrderFilterExtensions
{
    public static readonly Dictionary<string, Func<object, Expression<Func<Order, bool>>>> Options =
        new()
        {
            ["orderStatus"] = CreateOrderStatusFilter
        };
    private static Expression<Func<Order, bool>> CreateOrderStatusFilter(object value)
    {
        var raw = value?.ToString() ?? "";

        var statusList = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim().ToUpperInvariant())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        if (statusList.Count == 0)
            return _ => true;

        var validStatuses = statusList.Select(OrderStatus.Of).ToList();

        return order => validStatuses.Contains(order.OrderStatus);
    }

}