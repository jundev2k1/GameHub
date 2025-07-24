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
    public const string Staff = "3544228a-e12b-d7c9-da46-373340a7412f";
    public const string User = "fe000fef-f758-c67e-2bcf-617d059487c3";
}

public static class Seed
{
    public static async Task SeedData(
        IAsymmetricCryptoService cryptoService,
        UserManager<AppUser> userManager,
        GameXContext context)
    {
        // 系統角色
        if (!context.Roles.Any())
        {
            var roles = new List<IdentityRole>()
            {
                new()
                {
                    Id = RoleIds.Root,
                    Name = AppRoles.Root,
                    NormalizedName = AppRoles.Root.ToUpper(),
                },
                new()
                {
                    Id = RoleIds.Admin,
                    Name = AppRoles.Admin,
                    NormalizedName = AppRoles.Admin.ToUpper(),
                },
                new()
                {
                    Id = RoleIds.Staff,
                    Name = AppRoles.Staff,
                    NormalizedName = AppRoles.Staff.ToUpper(),
                },
                new()
                {
                    Id = RoleIds.User,
                    Name = AppRoles.User,
                    NormalizedName = AppRoles.User.ToUpper(),
                }
            };
            await context.Roles.AddRangeAsync(roles);
        }

        // 系統用戶
        if (!userManager.Users.Any())
        {
            var users = new List<AppUser>
            {
                new()
                {
                    Id = UserIds.Root,
                    Email = "root@example.com",
                    UserName = AppRoles.Root,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                },
            };

            foreach (var user in users)
            {
                if (user.UserName == "admin")
                {
                    await userManager.CreateAsync(user, "123456aA");
                    await userManager.AddToRoleAsync(user, AppRoles.Admin);
                }
                else if (user.UserName == AppRoles.Root)
                {
                    await userManager.CreateAsync(user, "GTlAWoc2K5BcmZ8Z");
                    await userManager.AddToRoleAsync(user, AppRoles.Root);
                }
            }
        }

        if (!context.AsymmetricKey.Any())
        {
            var (publicKeyPem, privateKeyPem) = cryptoService.GenerateKeyPair();

            context.AsymmetricKey.AddRange(new List<AsymmetricKey>
            {
                new()
                {
                    Name = AsymmetricKeyNames.GalaxyPay,
                    KeyType = KeyType.Private,
                    Algorithm = AsymmetricType.ECDSA,
                    KeyValue = privateKeyPem,
                    Description = "Galaxy Pay 系統簽名用私鑰"
                },
                new()
                {
                    Name = AsymmetricKeyNames.GalaxyPay,
                    KeyType = KeyType.Public,
                    Algorithm = AsymmetricType.ECDSA,
                    KeyValue = publicKeyPem,
                    Description = "Galaxy Pay 公鑰"
                },
                new()
                {
                    Name = AsymmetricKeyNames.Uxm,
                    KeyType = KeyType.Public,
                    Algorithm = AsymmetricType.ECDSA,
                    KeyValue = "-----BEGIN PUBLIC KEY-----\r\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEm1PhAmoUuAmQANNJFJov1Dra6kXt\r\nMM7OcxKGd0qtCZgNT375AasOYAKqxlhGZHX8ohfIF+Pa1bfbysSujYKGRw==\r\n-----END PUBLIC KEY-----",
                    Description = "Uxm 公鑰"
                }
            });

            await context.SaveChangesAsync();
        }

        await context.SaveChangesAsync();
    }
}
