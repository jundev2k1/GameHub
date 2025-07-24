using game_x.application.Features.UserRole.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface IRoleRepo
{
    Task<List<RoleDto>> GetAllAsync();
    Task<bool> IsValidUserAsync(string userId, string role,  CancellationToken ct = default);
}