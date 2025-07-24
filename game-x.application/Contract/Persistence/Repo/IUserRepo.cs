using game_x.domain.Identity;

namespace game_x.application.Contract.Persistence.Repo;

public interface IUserRepo
{
    Task<AppUser[]> GetUserByRole(string roleName, CancellationToken ct = default);

    Task<AppUser> GetUserByIdAsync(string userId, CancellationToken ct = default);

    Task<AppUser> GetUserByEmailAsync(string email, CancellationToken ct = default);

    Task<AppUser[]> GetAdminUsers(CancellationToken ct = default);

    Task<bool> IsExistEmailAsync(string email, CancellationToken ct = default);

    Task<bool> IsExistPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);

    Task AddUserAsync(AppUser user, string rawPassword, AppRole role, CancellationToken ct = default);

    Task UpdateAsync(string userId, Action<AppUser> updateAction, CancellationToken ct = default);
}
