using game_x.share.Extensions;
using System.Linq.Expressions;

namespace game_x.application.Extensions.FilterExtensions;

public static class BankAccountFilterExtensions
{
    public static readonly Dictionary<string, Func<object, Expression<Func<UserBankAccount, bool>>>> Options =
        new()
        {
            ["currencies"] = FilterByMultipleCurrencies,
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
}