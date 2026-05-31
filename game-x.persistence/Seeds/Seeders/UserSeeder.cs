using game_x.domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace game_x.persistence.Seeds.Seeders;

public sealed class UserSeeder(UserManager<User> userManager) : ISeeder
{
    private static class UserIds
    {
        public const string Root = "d32c7ae7-f676-b7cc-eabe-c39f56e755b1";
    }
    
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
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
}