using game_x.share.Extensions;
using System.Linq.Expressions;

namespace game_x.application.Extensions.FilterExtensions;

public static class LiveStreamFilterExtensions
{
    public static readonly Dictionary<string, Func<object, Expression<Func<LivestreamSchedule, bool>>>> Options =
        new()
        {
            ["statuses"] = FilterByMultipleStatuses,
        };

    private static Expression<Func<LivestreamSchedule, bool>> FilterByMultipleStatuses(object value)
    {
        var raw = value.ToStringOrEmpty();
        if (raw.IsNullOrWhiteSpace()) return _ => true;

        var statusList = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => Enum.TryParse<LiveStreamStatus>(s, true, out _))
            .Select(s => Enum.Parse<LiveStreamStatus>(s, true))
            .ToArray();
        if (statusList.Length == 0)
            return _ => false;

        return ls => statusList.Contains(ls.Status);
    }
}