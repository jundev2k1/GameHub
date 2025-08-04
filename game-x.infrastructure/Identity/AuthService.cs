using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Exceptions;
using game_x.share.Extensions;
using Microsoft.AspNetCore.Identity;

namespace game_x.infrastructure.Identity;

public sealed class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager) : IAuthService, IServices
{
    public async Task<User> TryLoginAsync(
        string userName,
        string password,
        bool rememberMe = false,
        bool shouldLockout = true)
    {
        var targetUser = await userManager.FindByNameAsync(userName) 
            ?? throw new BadRequestException(MessageCode.User.UserInvalidCredentials, $"Username ({userName}) is not found.");
        
        var loginResult = await signInManager.PasswordSignInAsync(userName, password, rememberMe, shouldLockout);
        if (loginResult.IsLockedOut)
        {
            var errorDetail = new { lockoutEnd = targetUser.LockoutEnd };
            throw new BadRequestException(MessageCode.User.UserLocked, errorDetail: errorDetail );
        }
        if (loginResult.IsNotAllowed)
            throw new BadRequestException(MessageCode.User.UserNotAllowed);
        if (loginResult.RequiresTwoFactor)
            throw new BadRequestException(MessageCode.User.UserRequiresTwoFactor);
        if (!loginResult.Succeeded)
            throw new BadRequestException(MessageCode.User.UserInvalidCredentials);

        return targetUser;
    }

    public async Task<AppRole> GetRolesAsync(User user)
    {
        var roles = await userManager.GetRolesAsync(user);
        return AppRole.Of(roles);
    }

    public async Task<bool> IsValidPasswordAsync(User user, string rawPassword, CancellationToken ct = default)
        => await userManager.CheckPasswordAsync(user, rawPassword);

    public async Task<string> GeneratePasswordResetTokenAsync(User user)
    {
        var result = await userManager.GeneratePasswordResetTokenAsync(user);
        return result;
    }

    public async Task ResetPasswordAsync(User user, string token, string newPassword, CancellationToken ct = default)
    {
        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errorMessage = result.Errors
                .Select(e => e.Description)
                .JoinToString(", ");
            throw new BadRequestException(
                MessageCode.User.UserResetPasswordFailed, errorMessage,
                errorMessage);
        }
    }

    public async Task ChangePasswordAsync(User user, string oldPassword, string newPassword)
    {
        var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        if (!result.Succeeded)
        {
            var errorMessage = result.Errors
                .Select(e => e.Description)
                .JoinToString(", ");
            throw new BadRequestException(
                MessageCode.User.UserChangePasswordFail,
                errorMessage);
        }
    }
}
