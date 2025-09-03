using game_x.share.Extensions;
using System.Linq.Expressions;

namespace game_x.application.Extensions.FilterExtensions;

public static class KycFilterExtensions
{
    public static readonly Dictionary<string, Func<object, Expression<Func<UserKyc, bool>>>> Options =
        new()
        {
            ["statuses"] = FilterByMultipleStatuses,
        };

    private static Expression<Func<UserKyc, bool>> FilterByMultipleStatuses(object value)
    {
        var rawStatuses = value.ToStringOrEmpty()
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (rawStatuses.Length == 0) return _ => true;

        var validStatuses = rawStatuses
            .Where(code => Enum.TryParse<KycStatus>(code, ignoreCase: true, out _))
            .Select(code => Enum.Parse<KycStatus>(code, ignoreCase: true))
            .ToArray();
        if (validStatuses.Length == 0) return _ => false;

        return kyc => validStatuses.Any(status => status == kyc.Status);
    }
}