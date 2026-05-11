namespace game_x.persistence.Seeds.Seeders;

public sealed class SystemWalletSeeder : ISeeder
{
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        var wallets = new[]
        {
            SystemWallet.Create(SystemWalletType.LiveStreamDonation),
        };

        foreach (var wallet in wallets)
        {
            var isExist = await context.SystemWallets
                .AsNoTracking()
                .AnyAsync(sw => sw.Type == wallet.Type, ct);
            if (isExist) continue;

            await context.SystemWallets.AddAsync(wallet, ct);
        }
    }
}