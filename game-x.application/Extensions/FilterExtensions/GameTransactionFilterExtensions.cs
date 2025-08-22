using System.Linq.Expressions;

namespace game_x.application.Extensions.FilterExtensions;

public static class GameTransactionFilterExtensions
{
    public static readonly Dictionary<string, Func<object, Expression<Func<GameTransaction, bool>>>> Options =
        new()
        {
            ["status"] = CreateStatusFilter
        };
    
    /// <summary>Builds a filter by multiple statuses.</summary>
    private static Expression<Func<GameTransaction, bool>> CreateStatusFilter(object value)
    {
        var raw = value.ToString() ?? "";

        var statusList = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim().ToUpperInvariant())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
        
        if (statusList.Count == 0)
            return _ => true;
        
        var validStatuses = statusList.Select(item =>
        {
            Enum.TryParse<GameTransactionStatus>(item, ignoreCase: true, out var status);
            return status;
        }).ToList();

        return transaction => validStatuses.Contains(transaction.Status);
    }
}