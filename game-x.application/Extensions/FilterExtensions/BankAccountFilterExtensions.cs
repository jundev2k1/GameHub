using game_x.share.Extensions;
using System.Linq.Expressions;

namespace game_x.application.Extensions.FilterExtensions;

public static class BankAccountFilterExtensions
{
    public static readonly Dictionary<string, Func<object, Expression<Func<UserBankAccount, bool>>>> Options =
        new()
        {
            ["currency"] = code =>
                ba => CurrencyUnit.IsValid(code.ToStringOrEmpty())
                    && ba.FiatCurrency.Code.Equals(CurrencyUnit.Of(code.ToStringOrEmpty())),
        };
}