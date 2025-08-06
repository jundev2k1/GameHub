using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.domain.Constants;
using game_x.share.Extensions;
using Microsoft.AspNetCore.Identity;

namespace game_x.persistence.Repo;

public sealed class UserRepo(GameXContext context, UserManager<User> userManager) : IUserRepo
{
    public async Task<User[]> GetUserByRole(string roleName, CancellationToken ct = default)
    {
        var result = await userManager.GetUsersInRoleAsync(roleName);
        return [.. result];
    }

    public async Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default)
    {
        var targetUser = await userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException(MessageCode.User.UserNotFound);

        var (isActive, errorCode) = targetUser.CheckValidUser();
        if (!isActive) throw new BadRequestException(errorCode!);

        return targetUser;
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken ct = default)
    {
        var targetUser = await userManager.FindByEmailAsync(email)
            ?? throw new NotFoundException(MessageCode.User.UserNotFound);

        var (isActive, errorCode) = targetUser.CheckValidUser();
        if (!isActive) throw new BadRequestException(errorCode!);

        return targetUser;
    }

    public async Task<User[]> GetAdminUsers(CancellationToken ct = default)
    {
        var users = await userManager.GetUsersInRoleAsync(AppRoles.Admin);
        return [.. users];
    }

    public async Task<UserKyc> GetKycProfileAsync(string userId, CancellationToken ct = default)
    {
        return await context.UserKycs
            .AsNoTracking()
            .Include(uk => uk.User)
            .Include(uk => uk.ReviewedBy)
            .Include(uk => uk.FrontImage)
            .Include(uk => uk.BackImage)
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.User.IsDeleted, ct)
            ?? throw new NotFoundException();
    }

    public async Task<(KycStatus Status, string? RejectionReason)> GetKycStatusAsync (string userId, CancellationToken ct = default)
    {
        var profile = await context.UserKycs
            .AsNoTracking()
            .Where(u => u.UserId == userId && !u.User.IsDeleted)
            .Select(u => Tuple.Create(u.Status, u.RejectionReason))
            .FirstOrDefaultAsync(ct);
        return profile != null
            ? (profile.Item1, profile.Item2)
            : (KycStatus.NotSubmitted, null);
    }

    public async Task<bool> IsExistEmailAsync(string email, CancellationToken ct = default)
        => await userManager.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower() && !u.IsDeleted, ct);

    public async Task<bool> IsExistPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
        => await userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber && !u.IsDeleted, ct);

    public async Task<bool> IsExistNicknameAsync(string nickname, CancellationToken ct = default)
        => await userManager.Users.AnyAsync(u => (u.Nickname.ToLower() == nickname.ToLower()) && !u.IsDeleted, ct);

    public async Task AddUserAsync(User user, string rawPassword, AppRole role, CancellationToken ct = default)
    {
        var userResult = await userManager.CreateAsync(user, rawPassword);
        if (!userResult.Succeeded)
        {
            var userError = userResult.Errors.Select(e => e.Description).JoinToString(", ");
            throw new BadRequestException($"Failed to add user: {userError}");
        }

        var roleResult = await userManager.AddToRolesAsync(user, role.Items);
        if (roleResult.Succeeded) return;

        var roleError = roleResult.Errors.Select(e => e.Description).JoinToString(", ");
        throw new BadRequestException($"Failed to add user to role: {roleError}");
    }

    public async Task UpdateAsync(string userId, Action<User> updateAction, CancellationToken ct = default)
    {
        var targetUser = await context.AppUsers
            .Include(u => u.UserKyc)
            .FirstOrDefaultAsync(user => user.Id == userId, ct)
            ?? throw new NotFoundException(MessageCode.User.UserNotFound);

        updateAction?.Invoke(targetUser);
    }
    public async Task UpdateByEmailAsync(string email, Action<User> updateAction, CancellationToken ct = default)
    {
        var targetUser = await userManager.FindByEmailAsync(email)
            ?? throw new NotFoundException(MessageCode.User.UserNotFound);

        updateAction?.Invoke(targetUser);
    }

    public async Task UpdateKycAsync(string userId, Action<UserKyc> updateAction, CancellationToken ct = default)
    {
        var targetKyc = await context.UserKycs
            .Include(uk => uk.FrontImage)
            .Include(uk => uk.BackImage)
            .FirstOrDefaultAsync(uk => uk.UserId == userId && !uk.User.IsDeleted, ct)
            ?? throw new NotFoundException(MessageCode.User.UserNotFound);

        updateAction?.Invoke(targetKyc);
    }
}
