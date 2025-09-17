using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.domain.Constants;
using game_x.share.Extensions;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace game_x.persistence.Repo;

public sealed class UserRepo(
    GameXContext context,
    UserManager<User> userManager,
    IFileManagerCacheService fileManagerCache) : IUserRepo, IRepository
{
    public async Task<User[]> GetUserByRole(string roleName, CancellationToken ct = default)
    {
        var result = await userManager.GetUsersInRoleAsync(roleName);
        return [.. result];
    }

    public async Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default)
    {
        var targetUser = await context.Users
            .Include(u => u.UserKyc)
            .Include(u => u.UserBankAccounts)
            .ThenInclude(ba => ba.FiatCurrency)
            .Include(u => u.UserExtend)
            .Include(u => u.UserRoles)
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ct)
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

    public async Task<User> GetAdminById(string userId, CancellationToken ct = default)
    {
        return await context.AppUsers
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && u.Status == UserStatus.Active, ct)
            ?? throw new NotFoundException(nameof(userId), userId);
    }

    public async Task<PaginationResult<User>> GetCsAdminByCriteria(
        Func<IQueryable<User>, IQueryable<User>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.AppUsers
            .AsNoTracking()
            .Where(u => u.UserRoles.Any(u => u.Role.Name == AppRoles.Cs))
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<User>(
            result,
            totalCount,
            totalPageCount,
            page,
            pageSize);
    }

    public async Task<UserDetailDto> GetUserDetailAsync(string userId, CancellationToken ct = default)
    {
        var targetUser = await context.Users
            .AsNoTracking()
            .Include(u => u.Avatar)
            .Include(u => u.UserKyc)
            .Include(u => u.UserExtend)
            .Include(u => u.UserBankAccounts)
            .Include(u => u.UserBalances)
                .ThenInclude(ub => ub.CryptoToken)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ct)
            ?? throw new NotFoundException();

        string? avatarUrl = null;
        if (targetUser.Avatar is not null)
        {
            var avatar = await fileManagerCache.GetImageUrl(targetUser.Avatar, ct);
            avatarUrl = avatar?.Url;
        }

        var result = targetUser.Adapt<UserDetailDto>();
        result.AvatarUrl = avatarUrl;
        return result;
    }

    public async Task<UserExtend> GetUserExtendAsync(string userId, CancellationToken ct = default)
    {
        return await context.UserExtends
            .AsNoTracking()
            .FirstOrDefaultAsync(ue => ue.Id == userId && !ue.User.IsDeleted, ct)
            ?? throw new NotFoundException(nameof(userId), userId);
    }

    public async Task<PaginationResult<UserKyc>> GetUserKycByCriteria(
        Func<IQueryable<UserKyc>, IQueryable<UserKyc>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.UserKycs
            .AsNoTracking()
            .Include(uk => uk.User)
            .Include(uk => uk.ReviewedBy)
            .Include(uk => uk.FrontImage)
            .Include(uk => uk.BackImage)
            .Where(uk => !uk.User.IsDeleted)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);
        return new PaginationResult<UserKyc>(
            items: result,
            totalItems: totalCount,
            totalPages: totalPageCount,
            pageIndex: page,
            pageSize: pageSize);
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

    public async Task<PaginationResult<UserBankAccount>> GetUserBankAccountByCriteria(
        Func<IQueryable<UserBankAccount>, IQueryable<UserBankAccount>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.UserBankAccounts
            .AsNoTracking()
            .Include(uba => uba.User)
            .Include(uba => uba.FiatCurrency)
            .Include(uba => uba.ReviewedBy)
            .Where(uba => !uba.User.IsDeleted)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<UserBankAccount>(
            items: result,
            totalItems: totalCount,
            totalPages: totalPageCount,
            pageIndex: page,
            pageSize: pageSize);
    }

    public async Task<(KycStatus Status, string? RejectionReason)> GetKycStatusAsync(string userId, CancellationToken ct = default)
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

    public async Task<VerificationStatusDto[]> GetVerificationStatusList(string userId, CancellationToken ct = default)
    {
        var user = await context.Users
            .AsNoTracking()
            .Include(u => u.UserKyc)
            .Include(u => u.UserBankAccounts)
            .ThenInclude(uba => uba.FiatCurrency)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ct)
            ?? throw new NotFoundException(MessageCode.User.UserNotFound);
        var supportedCurrencies = await context.FiatCurrencies
            .AsNoTracking()
            .Where(fc => fc.IsActive)
            .ToListAsync(ct);

        var kycStatus = user.UserKyc != null
            ? user.UserKyc.Adapt<VerificationStatusDto>()
            : new VerificationStatusDto
            {
                Type = VerificationStatusType.Kyc,
                IsVerified = false,
            };

        var bankAccountStatuses = supportedCurrencies
            .GroupJoin(
                user.UserBankAccounts,
                fc => fc.Id,
                uba => uba.FiatCurrency.Id,
                (fc, ubas) =>
                {
                    var uba = ubas.FirstOrDefault();
                    if (uba != null) return uba.Adapt<VerificationStatusDto>();

                    var defaultValue = new VerificationStatusDto
                    {
                        CurrencyCode = fc.Code.Value,
                        Type = VerificationStatusType.BankAccount,
                        IsVerified = false,
                    };
                    return defaultValue;
                })
            .ToArray();
        return [kycStatus, .. bankAccountStatuses];
    }
    public async Task<PaginationResult<UserDto>> GetUserByCriteriaAsync(
        Func<IQueryable<UserDto>, IQueryable<UserDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var baseQuery = userManager.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => !u.IsDeleted &&
                        u.UserRoles.Any(ur => ur.Role.NormalizedName == AppRoles.User.ToUpper()))
            .AsQueryable();

        var query = baseQuery
            .Select(u => new UserDto
            {
                Id = u.Id,
                Nickname = u.Nickname,
                UserName = u.UserName!,
                Email = u.Email!,
                Status = u.Status,
                CountryCode = u.CountryCode,
                EmailConfirmed = u.EmailConfirmed,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<UserDto>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<bool> IsExistUserIdAsync(string userId, CancellationToken ct = default)
        => await userManager.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted, ct);

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
            .Include(u => u.UserBankAccounts)
            .ThenInclude(uba => uba.FiatCurrency)
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
