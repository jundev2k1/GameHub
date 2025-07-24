using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.UserRole.Dtos;
using game_x.domain.Constants;
using Mapster;

namespace game_x.persistence.Repo;

public sealed class RoleRepo(GameXContext context) : IRoleRepo
{
    public async Task<List<RoleDto>> GetAllAsync()
    {
        var roles = await context.Roles
            .Where(r => r.Name != AppRoles.Root)
            .Select(r => r.Adapt<RoleDto>())
            .ToListAsync();
        return roles;
    }

    public async Task<bool> IsValidUserAsync(string userId, string role, CancellationToken ct = default)
    {
        var lowerRole = role.ToLowerInvariant();
        return await context.AppUserRole
            .AnyAsync(r => r.User.Id == userId && r.Role.Name == lowerRole && !r.User.IsDeleted, ct);
    }
}
