using game_x.domain.Constants;

namespace game_x.persistence.Seeds.Seeders;

public sealed class RoleSeeder : ISeeder
{
    private static class RoleIds
    {
        public const string Root = "99b9cfef-1e02-cac0-abf6-b87a6e95bd48";
        public const string Admin = "64856429-39cc-2cb0-427e-c6a6549cf10a";
        public const string Cs = "3544228a-e12b-d7c9-da46-373340a7412f";
        public const string Talent = "bdbaff6d-7c4b-4426-bdff-6db9d29b39ea";
        public const string User = "fe000fef-f758-c67e-2bcf-617d059487c3";
    }
    
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
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
            if (await context.Roles
                    .AsNoTracking()
                    .AnyAsync(r => r.Id == role.Id, ct))
                continue;

            await context.Roles.AddAsync(role, ct);
        }
    }
}