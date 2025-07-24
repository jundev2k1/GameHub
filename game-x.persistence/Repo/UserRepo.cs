using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Constants;
using game_x.domain.Enum;
using game_x.share.Extensions;
using Microsoft.AspNetCore.Identity;

namespace game_x.persistence.Repo;

public sealed class UserRepo(GameXContext context, UserManager<AppUser> userManager) : IUserRepo
{
    public async Task<UserStatisticsDto> GetUserStatisticsAsync(CancellationToken ct = default)
    {
        var userRoleId = await context.Roles
            .Where(r => r.Name == AppRoles.User)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(ct);

        var userIds = await context.UserRoles
            .Where(ur => ur.RoleId == userRoleId)
            .Select(ur => ur.UserId)
            .ToListAsync(ct);

        var query = context.AppUser
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id));

        var totalUsers = await query.CountAsync(ct);
        var activeUsers = await query.CountAsync(u => u.Status == AppUserStatus.Active, ct);
        var inactiveUsers = await query.CountAsync(u => u.Status == AppUserStatus.Inactive, ct);
        var newUsers = await query.CountAsync(u => u.IsNew, ct);

        return new UserStatisticsDto
        {
            AllUser = totalUsers,
            ActiveUser = activeUsers,
            InactiveUser = inactiveUsers,
            NewUser = newUsers
        };
    }

    public async Task<AppUser[]> GetUserByRole(string roleName, CancellationToken ct = default)
    {
        var result = await userManager.GetUsersInRoleAsync(roleName);
        return [.. result];
    }

    public async Task<AppUser> GetUserByIdAsync(string userId, CancellationToken ct = default)
    {
        var targetUser = await userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException(MessageCode.User.UserNotFound);

        var (isActive, errorCode) = targetUser.CheckValidUser();
        if (!isActive) throw new BadRequestException(errorCode!);

        return targetUser;
    }

    public async Task<AppUser> GetUserByEmailAsync(string email, CancellationToken ct = default)
    {
        var targetUser = await userManager.FindByEmailAsync(email)
            ?? throw new NotFoundException(MessageCode.User.UserNotFound);

        var (isActive, errorCode) = targetUser.CheckValidUser();
        if (!isActive) throw new BadRequestException(errorCode!);

        return targetUser;
    }

    public async Task<AppUser[]> GetAdminUsers(CancellationToken ct = default)
    {
        var users = await userManager.GetUsersInRoleAsync(AppRoles.Admin);
        return [.. users];
    }

    public async Task<bool> IsExistEmailAsync(string email, CancellationToken ct = default)
        => await userManager.Users.AnyAsync(u => u.Email == email && !u.IsDeleted, ct);

    public async Task<bool> IsExistPhoneNumberAsync(string phoneNumber, CancellationToken ct = default)
        => await userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber && !u.IsDeleted, ct);

    public async Task AddUserAsync(AppUser user, string rawPassword, AppRole role, CancellationToken ct = default)
    {
        var userResult = await userManager.CreateAsync(user, rawPassword);
        if (!userResult.Succeeded)
        {
            var errors = userResult.Errors
                .Select(e => e.Description)
                .JoinToString(", ");
            throw new BadRequestException($"Failed to add user: {errors}");
        }

        foreach (var roleName in role.Items)
        {
            var roleResult = await userManager.AddToRoleAsync(user, roleName);
            if (!roleResult.Succeeded)
            {
                var errors = roleResult.Errors
                    .Select(e => e.Description)
                    .JoinToString(", ");
                throw new BadRequestException($"Failed to add user to role '{roleName}': {errors}");
            }
        }
    }

    public async Task UpdateAsync(string userId, Action<AppUser> updateAction, CancellationToken ct = default)
    {
        var targetUser = await context.AppUser
            .FirstOrDefaultAsync(user => user.Id == userId, ct)
            ?? throw new NotFoundException(MessageCode.User.UserNotFound);

        updateAction?.Invoke(targetUser);
    }
}
