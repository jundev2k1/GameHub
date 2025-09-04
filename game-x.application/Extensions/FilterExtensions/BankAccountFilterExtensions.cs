using game_x.share.Extensions;
using System.Linq.Expressions;

namespace game_x.application.Extensions.FilterExtensions;

public static class BankAccountFilterExtensions
{
    public static readonly Dictionary<string, Func<object, Expression<Func<UserBankAccount, bool>>>> Options =
        new()
        {
            ["currencies"] = FilterByMultipleCurrencies,
            ["statuses"] = FilterByMultipleStatuses,
        };

    private static Expression<Func<UserBankAccount, bool>> FilterByMultipleCurrencies(object value)
    {
        var rawCodes = value.ToStringOrEmpty()
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (rawCodes.Length == 0) return _ => true;

        var validCodes = rawCodes
            .Where(CurrencyUnit.IsValid)
            .Select(CurrencyUnit.Of)
            .ToArray();
        if (validCodes.Length == 0) return _ => false;

        return ba => validCodes.Any(currency => currency.Equals(ba.FiatCurrency.Code));
    }

    private static Expression<Func<UserBankAccount, bool>> FilterByMultipleStatuses(object value)
    {
        var rawStatuses = value.ToStringOrEmpty()
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (rawStatuses.Length == 0) return _ => true;

        var validStatuses = rawStatuses
            .Where(status => Enum.TryParse<UserBankAccountStatus>(status, ignoreCase: true, out _))
            .Select(status => Enum.Parse<UserBankAccountStatus>(status, ignoreCase: true))
            .ToArray();
        if (validStatuses.Length == 0) return _ => false;

        return ba => validStatuses.Contains(ba.Status);
    }
}