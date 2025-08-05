namespace game_x.application.Contract.Persistence.Identity;

public interface IAuthService
{
    Task<User> TryLoginAsync(string userName, string password, bool rememberMe = false, bool shouldLockout = true);

    Task<AppRole> GetRolesAsync(User user);

    Task<bool> IsValidPasswordAsync(User user, string rawPassword, CancellationToken ct = default);

    Task<string> GeneratePasswordResetTokenAsync(User user);

    Task ResetPasswordAsync(User user, string token, string newPassword, CancellationToken ct = default);

    Task ChangePasswordAsync(User user, string oldPassword, string newPassword);
}
