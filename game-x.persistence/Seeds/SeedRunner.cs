using game_x.application.Contract.Infrastructure.Security;
using game_x.persistence.Seeds.Seeders;
using Microsoft.AspNetCore.Identity;

namespace game_x.persistence.Seeds;

public static class SeedRunner
{
    public static async Task RunAsync(
        GameXContext context,
        UserManager<User> userManager, 
        IAsymmetricCryptoService cryptoService)
    {
        await new AppSettingSeeder().SeedAsync(context);
        
        await new RoleSeeder().SeedAsync(context);
        await new UserSeeder(userManager).SeedAsync(context);
        
        await new SystemWalletSeeder().SeedAsync(context);
        
        await new AsymmetricKeySeeder(cryptoService).SeedAsync(context);
        await new CryptoTokenSeeder().SeedAsync(context);
        await new FiatCurrencySeeder().SeedAsync(context);
        
        await new GamePlatformSeeder().SeedAsync(context);
        await new GameCategorySeeder().SeedAsync(context);
        await new GameTypeSeeder().SeedAsync(context);
        await new GameSeeder().SeedAsync(context);
        
        await new ConversationSeeder().SeedAsync(context);
        
        await new RewardPoolSeeder().SeedAsync(context);
        await context.SaveChangesAsync();
    }
}