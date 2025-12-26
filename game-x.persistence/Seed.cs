using game_x.application.Contract.Infrastructure.Security;
using game_x.domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace game_x.persistence;

public static class UserIds
{
    public const string Root = "d32c7ae7-f676-b7cc-eabe-c39f56e755b1";
}

public static class RoleIds
{
    public const string Root = "99b9cfef-1e02-cac0-abf6-b87a6e95bd48";
    public const string Admin = "64856429-39cc-2cb0-427e-c6a6549cf10a";
    public const string Cs = "3544228a-e12b-d7c9-da46-373340a7412f";
    public const string Talent = "bdbaff6d-7c4b-4426-bdff-6db9d29b39ea";
    public const string User = "fe000fef-f758-c67e-2bcf-617d059487c3";
}

public static class Seed
{
    public static async Task SeedData(
        IAsymmetricCryptoService cryptoService,
        UserManager<User> userManager,
        GameXContext context)
    {
        // Seed app setting
        await SeedAppSettings(context);

        // Seed roles and users
        await SeedRoles(context);
        await SeedUsers(userManager);

        // Seed system wallets
        await SeedSystemWallet(context);

        // Seed asymmetric keys and crypto tokens
        await SeedAsymmetricKeys(cryptoService, context);
        await SeedCryptoTokens(context);
        await SeedFiatCurrencies(context);

        // Seed game platforms
        await SeedGamePlatforms(context);
        await SeedGameCategories(context);
        await SeedGameTypes(context);
        await SeedGames(context);

        // Seed public conversation
        await SeedPublicConversation(context);

        // Save changes to the database
        await context.SaveChangesAsync();
    }

    private static async Task SeedAppSettings(GameXContext context)
    {
        var settings = new AppSetting[]
        {
            AppSetting.Create(AppSettingConstant.KEY_TALENT_COMMISSION_RATE, "70", string.Empty, true),
        };

        foreach (var setting in settings)
        {
            var isExist = await context.AppSettings.AsNoTracking().AnyAsync(sw => sw.Key == setting.Key);
            if (isExist) continue;

            await context.AppSettings.AddAsync(setting);
        }
    }

    private static async Task SeedRoles(GameXContext context)
    {
        var roles = new List<Role>()
        {
            Role.Create(AppRoles.Root, RoleIds.Root),
            Role.Create(AppRoles.Admin, RoleIds.Admin),
            Role.Create(AppRoles.Cs, RoleIds.Cs),
            Role.Create(AppRoles.Talent, RoleIds.Talent),
            Role.Create(AppRoles.User, RoleIds.User),
        };
        foreach (var role in roles)
        {
            if (await context.Roles.AsNoTracking().AnyAsync(r => r.Id == role.Id))
                continue;

            await context.Roles.AddAsync(role);
        }
    }

    private static async Task SeedUsers(UserManager<User> userManager)
    {
        if (userManager.Users.Any()) return;

        static User CreateSeedUser(string id, string userName, string email)
        {
            var user = User.Create(userName, email);
            user.Id = id;
            user.ConfirmEmail();
            user.ConfirmPhoneNumber();
            return user;
        }

        var users = new List<User>
        {
            // Seed: root user
            CreateSeedUser(UserIds.Root, AppRoles.Root, "root@example.com"),
        };
        foreach (var user in users)
        {
            if (user.UserName == AppRoles.Admin)
            {
                await userManager.CreateAsync(user, "Password123@");
                await userManager.AddToRoleAsync(user, AppRoles.Admin);
            }
            else if (user.UserName == AppRoles.Root)
            {
                await userManager.CreateAsync(user, "GTlAWoc2K5BcmZ8Z");
                await userManager.AddToRoleAsync(user, AppRoles.Root);
            }
        }
    }

    private static async Task SeedSystemWallet(GameXContext context)
    {
        var wallets = new SystemWallet[]
        {
            SystemWallet.Create(SystemWalletType.LiveStreamDonation),
        };

        foreach (var wallet in wallets)
        {
            var isExist = await context.SystemWallets.AsNoTracking().AnyAsync(sw => sw.Type == wallet.Type);
            if (isExist) continue;

            await context.SystemWallets.AddAsync(wallet);
        }
    }

    private static async Task SeedAsymmetricKeys(IAsymmetricCryptoService cryptoService, GameXContext context)
    {
        if (await context.AsymmetricKeys.AnyAsync()) return;

        var (publicKeyPem, privateKeyPem) = cryptoService.GenerateKeyPair();

        var keys = new List<AsymmetricKey>
        {
            AsymmetricKey.Create(
                AsymmetricKeyNames.GameX,
                AsymmetricKeyType.Private,
                AsymmetricType.ECDSA,
                privateKeyPem,
                "GameX 系統簽名用私鑰"),
            AsymmetricKey.Create(
                AsymmetricKeyNames.GameX,
                AsymmetricKeyType.Public,
                AsymmetricType.ECDSA,
                publicKeyPem,
                "GameX 公鑰"),
            AsymmetricKey.Create(
                AsymmetricKeyNames.GalaxyPay,
                AsymmetricKeyType.Public,
                AsymmetricType.ECDSA,
                "----- BEGIN PUBLIC KEY-----\r\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE3wb6KaEwU9zFGqn6/BmH7T+Wekcs\r\nEMi2RnMWLztrnfF3Ck0O8G5s88jm0Zhq15t9W7VA0Qu3sr/6WFoZjS2c7g==\r\n-----END PUBLIC KEY-----",
                "Galaxy Pay 公鑰"),
            AsymmetricKey.Create(
                AsymmetricKeyNames.Uxm,
                AsymmetricKeyType.Public,
                AsymmetricType.ECDSA,
                "----- BEGIN PUBLIC KEY-----\r\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEm1PhAmoUuAmQANNJFJov1Dra6kXt\r\nMM7OcxKGd0qtCZgNT375AasOYAKqxlhGZHX8ohfIF+Pa1bfbysSujYKGRw==\r\n-----END PUBLIC KEY-----",
                "Uxm 公鑰"),
        };

        await context.AsymmetricKeys.AddRangeAsync(keys);
    }

    private static async Task SeedCryptoTokens(GameXContext context)
    {
        if (await context.CryptoTokens.AnyAsync()) return;

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

        await context.CryptoTokens.AddRangeAsync(cryptoTokens);
    }

    private static async Task SeedFiatCurrencies(GameXContext context)
    {
        if (await context.FiatCurrencies.AnyAsync()) return;

        var fiatCurrencies = new List<FiatCurrency>
        {
            FiatCurrency.Create(CurrencyUnit.Of("TWD"), "New Taiwan Dollar", "NT$", string.Empty, false),
            FiatCurrency.Create(CurrencyUnit.Of("USD"), "US Dollar", "$", string.Empty, false),
            FiatCurrency.Create(CurrencyUnit.Of("CNY"), "Chinese Yuan", "¥", string.Empty),
            FiatCurrency.Create(CurrencyUnit.Of("VND"), "Vietnamese Dong", "₫", string.Empty),
        };

        await context.FiatCurrencies.AddRangeAsync(fiatCurrencies);
    }

    private static async Task SeedGamePlatforms(GameXContext context)
    {
        var gamePlatforms = new GamePlatform[]
        {
            GamePlatform.Create("598彩票", string.Empty, string.Empty, 0, GameConstants.PLATFORM_ID_G598),
            GamePlatform.Create("百家樂", string.Empty, string.Empty, 0, GameConstants.PLATFORM_ID_GAMEBACCARAT),
            GamePlatform.Create("etl998", string.Empty, string.Empty, 0, GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT),
            GamePlatform.Create("SAS Slot", string.Empty, string.Empty, 0, GameConstants.PLATFORM_ID_GAMESLOT),
        };
        foreach (var gamePlatform in gamePlatforms)
        {
            var isExist = await context.GamePlatforms
                .AnyAsync(p => p.PublicId == gamePlatform.PublicId);
            if (isExist) continue;

            await context.GamePlatforms.AddAsync(gamePlatform);
        }
    }

    private static readonly Dictionary<string, Guid> GameCategories = new()
    {
        { "彩票", Guid.Parse("9f1f6c6e-25b5-4d40-9a07-8de1e56afec4") },
    };

    private static async Task SeedGameCategories(GameXContext context)
    {
        if (await context.GameCategories.AnyAsync()) return;

        var gameCategories = new GameCategory[]
        {
            GameCategory.Create("彩票", "Lottery", string.Empty, 0, GameCategories["彩票"])
        };
        await context.GameCategories.AddRangeAsync(gameCategories);
    }

    private static readonly Dictionary<string, Guid> GameTypes = new()
    {
        { "快三", Guid.Parse("4bafec6b-68d3-4f20-832d-3e9f4a1d1e25") },
        { "PK10", Guid.Parse("2f7e94a6-69ab-4f64-8b30-9fd563c60b41") },
        { "PK6", Guid.Parse("67d3a8f4-8fb2-4e84-9b52-1c9f82c1f46a") },
        { "時時彩", Guid.Parse("a7f91d64-058b-4b7e-88f4-d78a0b493d12") },
        { "六合彩", Guid.Parse("e3f9d6c7-1b5a-4629-bc8e-2f6b1d4c93e0") },
        { "3D", Guid.Parse("51a8b2d6-2982-4a7e-9c2f-7f38e1a0d1f5") },
        { "蛋蛋", Guid.Parse("8e9b1f62-4d38-4f7a-bc19-2e3a1b4f8c7d") },
    };

    private static async Task SeedGameTypes(GameXContext context)
    {
        if (await context.GameTypes.AnyAsync()) return;

        var gameTypes = new List<GameType>
        {
            GameType.Create("快三", string.Empty, string.Empty, 0, GameTypes["快三"]),
            GameType.Create("PK10", string.Empty, string.Empty, 0, GameTypes["PK10"]),
            GameType.Create("PK6", string.Empty, string.Empty, 0, GameTypes["PK6"]),
            GameType.Create("時時彩", string.Empty, string.Empty, 0, GameTypes["時時彩"]),
            GameType.Create("六合彩", string.Empty, string.Empty, 0, GameTypes["六合彩"]),
            GameType.Create("3D", string.Empty, string.Empty, 0, GameTypes["3D"]),
            GameType.Create("蛋蛋", string.Empty, string.Empty, 0, GameTypes["蛋蛋"]),
        };
        await context.GameTypes.AddRangeAsync(gameTypes);
    }

    private static async Task SeedGames(GameXContext context)
    {
        async Task CreateEntity(string name, string code, Guid platformId)
        {
            var isExist = await context.Games.AnyAsync(g => g.Platform.PublicId == platformId && g.GameCode == code);
            if (isExist) return;

            var entity = Game.Create(name, code, string.Empty, string.Empty, 0);
            var platform = await context.GamePlatforms
                .FirstOrDefaultAsync(p => p.PublicId == platformId);
            if (platform == null)
            {
                var localPlatform = context.GamePlatforms.Local
                    .FirstOrDefault(p => p.PublicId == platformId);
                if (localPlatform != null)
                    entity.UpdatePlatform(localPlatform);
            }
            else
            {
                entity.UpdatePlatform(platform);
            }

            await context.AddAsync(entity);
        }

        // Seed game list for 598彩票
        await CreateEntity("韓國快3", "KRF3", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("極速快3", "ESF3", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("澳洲幸運10", "ALPK10", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("極速賽車", "ESPK10", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("幸運飛艇", "LBPK10", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("極速飛艇", "EBPK10", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("動物運動會", "ANPK6", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("三分動物運動會", "M3ANPK6", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("極速時時彩", "ESSSC", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("澳洲幸運5", "ALSSC", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("幸運時時彩", "LBSSC", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("香港六合彩", "HKM6", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("澳門六合彩", "MCM6", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("新澳門六合彩", "NMCM6", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("台灣大樂透", "TWM6", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("香港日日六合彩", "HKDM6", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("福彩3D", "FCD3", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("體彩排列3", "PLD3", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("PC蛋蛋", "ALEGG", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("澳洲幸運10番攤", "AL10FT", GameConstants.PLATFORM_ID_G598);
        await CreateEntity("澳洲幸運5番攤", "AL5FT", GameConstants.PLATFORM_ID_G598);

        // Seed game list for 百家樂
        await CreateEntity("百家樂", "BAC001", GameConstants.PLATFORM_ID_GAMEBACCARAT);

        // Seed game list for ETL998
        await CreateEntity("998百家樂", "BAC002", GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT);

        // Seed game list for SAS Slot
        await CreateEntity("Slot", "SLOT001", GameConstants.PLATFORM_ID_GAMESLOT);
    }

    private static async Task SeedPublicConversation(GameXContext db, CancellationToken ct = default)
    {
        var exists = await db.Conversations.AnyAsync(c => c.Type == ConversationType.Public, ct);
        if (!exists)
        {
            var conv = Conversation.Create(type: ConversationType.Public);
            db.Conversations.Add(conv);
        }
    }
}
