using game_x.domain.Identity;

namespace game_x.application.Contract.Persistence.Identity;

public interface IAuthService
{
    Task<AppUser> TryLoginAsync(string userName, string password, bool rememberMe = false, bool shouldLockout = true);

    Task<AppRole> GetRolesAsync(AppUser user);

    Task<bool> IsValidPasswordAsync(AppUser user, string rawPassword, CancellationToken ct = default);

    Task<string> GeneratePasswordResetTokenAsync(AppUser user, CancellationToken ct = default);

    Task ResetPasswordAsync(AppUser user, string token, string newPassword, CancellationToken ct = default);

    Task ChangePasswordAsync(AppUser user, string oldPassword, string newPassword, CancellationToken ct = default);
}
