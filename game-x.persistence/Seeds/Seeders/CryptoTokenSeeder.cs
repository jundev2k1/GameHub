using game_x.domain.Constants;

namespace game_x.persistence.Seeds.Seeders;

public sealed class CryptoTokenSeeder : ISeeder
{
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        if (await context.CryptoTokens.AnyAsync(ct)) return;

        var cryptoTokens = new List<CryptoToken>
        {
            new()
            {
                Symbol = CryptoTokenSymbol.Usdt,
                Network = NetworkType.Tron,
                ContractAddress = "trc20-ContractAddress",
                Status = CryptoTokenStatus.Active,
            },
            new()
            {
                Symbol = CryptoTokenSymbol.Usdt,
                Network = NetworkType.Ethereum,
                ContractAddress = "erc20-ContractAddress",
                Status = CryptoTokenStatus.Inactive
            },
        };

        await context.CryptoTokens.AddRangeAsync(cryptoTokens, ct);
    }
}