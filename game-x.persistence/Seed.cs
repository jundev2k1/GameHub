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
    public const string User = "fe000fef-f758-c67e-2bcf-617d059487c3";
}

public static class Seed
{
    public static async Task SeedData(
        IAsymmetricCryptoService cryptoService,
        UserManager<User> userManager,
        GameXContext context)
    {
        if (!context.Roles.Any())
        {
            var roles = new List<Role>()
            {
                Role.Create(AppRoles.Root, RoleIds.Root),
                Role.Create(AppRoles.Admin, RoleIds.Admin),
                Role.Create(AppRoles.Cs, RoleIds.Cs),
                Role.Create(AppRoles.User, RoleIds.User),
            };
            await context.Roles.AddRangeAsync(roles);
        }

        if (!userManager.Users.Any())
        {
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

        if (!context.AsymmetricKeys.Any())
        {
            var (publicKeyPem, privateKeyPem) = cryptoService.GenerateKeyPair();

            context.AsymmetricKeys.AddRange(new List<AsymmetricKey>
            {
                AsymmetricKey.Create(
                    name: AsymmetricKeyNames.GameX,
                    keyType: AsymmetricKeyType.Private,
                    algorithm: AsymmetricType.ECDSA,
                    value: privateKeyPem,
                    desc: "GameX 系統簽名用私鑰"),
                AsymmetricKey.Create(
                    name: AsymmetricKeyNames.GameX,
                    keyType: AsymmetricKeyType.Public,
                    algorithm: AsymmetricType.ECDSA,
                    value: publicKeyPem,
                    desc: "GameX 公鑰"),
                AsymmetricKey.Create(
                    name: AsymmetricKeyNames.GalaxyPay,
                    keyType: AsymmetricKeyType.Public,
                    algorithm: AsymmetricType.ECDSA,
                    value: "----- BEGIN PUBLIC KEY-----\r\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE3wb6KaEwU9zFGqn6/BmH7T+Wekcs\r\nEMi2RnMWLztrnfF3Ck0O8G5s88jm0Zhq15t9W7VA0Qu3sr/6WFoZjS2c7g==\r\n-----END PUBLIC KEY-----",
                    desc: "Galaxy Pay 公鑰"),
                AsymmetricKey.Create(
                    name: AsymmetricKeyNames.Uxm,
                    keyType: AsymmetricKeyType.Public,
                    algorithm: AsymmetricType.ECDSA,
                    value: "----- BEGIN PUBLIC KEY-----\r\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEm1PhAmoUuAmQANNJFJov1Dra6kXt\r\nMM7OcxKGd0qtCZgNT375AasOYAKqxlhGZHX8ohfIF+Pa1bfbysSujYKGRw==\r\n-----END PUBLIC KEY-----",
                    desc: "Uxm 公鑰"),
            });
        }
        
        // Cryptocurrency
        if (!context.CryptoTokens.Any())
        {
            var cryptoTokens = new List<CryptoToken>
            {
                new()
                {
                    Symbol = CryptoTokenSymbol.Usdt,
                    Network = NetworkType.Tron,
                    ContractAddress = "trc20-ContractAddress"
                },
                new()
                {
                    Symbol = CryptoTokenSymbol.Usdt,
                    Network = NetworkType.Ethereum,
                    ContractAddress = "erc20-ContractAddress"
                },
            };
            await context.CryptoTokens.AddRangeAsync(cryptoTokens);
        }

        if (!context.FiatCurrencies.Any())
        {
            var fiatCurrencies = new List<FiatCurrency>
            {
                FiatCurrency.Create(CurrencyUnit.Of("TWD"), "New Taiwan Dollar", "NT$", string.Empty, false),
                FiatCurrency.Create(CurrencyUnit.Of("USD"), "US Dollar", "$", string.Empty, false),
                FiatCurrency.Create(CurrencyUnit.Of("CNY"), "Chinese Yuan", "¥", string.Empty),
                FiatCurrency.Create(CurrencyUnit.Of("VND"), "Vietnamese Dong", "₫", string.Empty),
            };
            await context.FiatCurrencies.AddRangeAsync(fiatCurrencies);
        }

        await context.SaveChangesAsync();
    }
}
