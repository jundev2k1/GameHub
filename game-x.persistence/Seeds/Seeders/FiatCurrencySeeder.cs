namespace game_x.persistence.Seeds.Seeders;

public sealed class FiatCurrencySeeder : ISeeder
{
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        if (await context.FiatCurrencies.AnyAsync(ct)) return;

        var fiatCurrencies = new List<FiatCurrency>
        {
            FiatCurrency.Create(CurrencyUnit.Of("TWD"), "New Taiwan Dollar", "NT$", string.Empty, false),
            FiatCurrency.Create(CurrencyUnit.Of("USD"), "US Dollar", "$", string.Empty, false),
            FiatCurrency.Create(CurrencyUnit.Of("CNY"), "Chinese Yuan", "¥", string.Empty),
            FiatCurrency.Create(CurrencyUnit.Of("VND"), "Vietnamese Dong", "₫", string.Empty),
        };

        await context.FiatCurrencies.AddRangeAsync(fiatCurrencies, ct);
    }
}