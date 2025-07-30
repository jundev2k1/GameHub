namespace game_x.application.Contract.Persistence.Repo;

public interface IUserRepo
{
    Task<User[]> GetUserByRole(string roleName, CancellationToken ct = default);

    Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default);

    Task<User> GetUserByEmailAsync(string email, CancellationToken ct = default);

    Task<User[]> GetAdminUsers(CancellationToken ct = default);

    Task<UserKyc> GetKycProfile(string userId, CancellationToken ct = default);

    Task<bool> IsExistEmailAsync(string email, CancellationToken ct = default);

    Task<bool> IsExistPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);

    Task<bool> IsExistNicknameAsync(string nickname, CancellationToken ct = default);

    Task AddUserAsync(User user, string rawPassword, AppRole role, CancellationToken ct = default);

    Task UpdateAsync(string userId, Action<User> updateAction, CancellationToken ct = default);

    Task UpdateByEmailAsync(string email, Action<User> updateAction, CancellationToken ct = default);
}
