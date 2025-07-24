using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace game_x.persistence.Repo;

public sealed class AppUserRepo(
    UserManager<AppUser> userManager,
    GameXContext context) : IAppUserRepo
{
    public async Task<PaginationResult<UserMappingDto>> GetUserByCriteriaAsync(
        Func<IQueryable<UserMappingDto>, IQueryable<UserMappingDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var baseQuery = userManager.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.Passport)
            .Include(u => u.StaffUser)
            .Where(u => !u.IsDeleted &&
                        u.UserRoles.Any(ur => ur.Role.NormalizedName == AppRoles.User.ToUpper())).AsQueryable();

        var query = baseQuery
            .GroupJoin(
                context.StaffUsers,
                user => user.Id,
                su => su.UserId,
                (user, suGroup) => new { user, suGroup }
            )
            .SelectMany(
                x => x.suGroup.DefaultIfEmpty(),
                (x, su) => new { x.user, su }
            )
            .GroupJoin(
                userManager.Users,
                x => x.su != null ? x.su.StaffId : null,
                staff => staff.Id,
                (x, staffGroup) => new { x.user, x.su, staffGroup }
            )
            .SelectMany(
                x => x.staffGroup.DefaultIfEmpty(),
                (x, staff) => new { x.user, x.su, staff }
            )
            .Select(x => new UserMappingDto
            {
                Id = x.user.Id,
                UserName = x.user.UserName,
                Email = x.user.Email,
                PhoneNumber = x.user.PhoneNumber,
                Passport = x.user.Passport,
                Status = x.user.Status,
                CountryCode = x.user.CountryCode,
                CreatedAt = x.user.CreatedAt,
                UpdatedAt = x.user.UpdatedAt,
                IsNew = x.user.IsNew,
                StaffUser = x.user.StaffUser,
                UserRoles = x.user.UserRoles,
                Staff = x.staff
            });

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<UserMappingDto>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<AppUser?> GetUserDetailByIdAsync(string userId, CancellationToken ct = default)
    {
        return await userManager.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.Passport)
            .ThenInclude(p => p.PassportImage)
            .Include(u => u.StaffUser)
            .ThenInclude(su => su != null ? su.Staff : null)
            .Where(u => !u.IsDeleted && u.Id == userId)
            .FirstOrDefaultAsync(u => u.UserRoles.Any(ur => ur.Role.Name == AppRoles.User), ct);
    }

    public async Task<AppUser?> GetStaffDetailByIdAsync(string staffId, CancellationToken ct = default)
    {
        return await userManager.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.StaffExtension)
            .ThenInclude(se => se != null ? se.Admin : null)
            .Where(u => !u.IsDeleted && u.Id == staffId)
            .FirstOrDefaultAsync(u => u.UserRoles.Any(ur => ur.Role.Name == AppRoles.Staff), ct);
    }

    public async Task<AppUser?> GetAdminByIdAsync(string adminId, CancellationToken ct = default)
    {
        return await userManager.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => !u.IsDeleted && u.Id == adminId)
            .FirstOrDefaultAsync(u =>
                u.UserRoles.Any(ur => ur.Role.Name == AppRoles.Admin), ct);
    }

    public async Task<PaginationResult<StaffMappingDto>> GetStaffByCriteriaAsync(
        Func<IQueryable<StaffMappingDto>, IQueryable<StaffMappingDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var baseQuery = userManager.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.StaffExtension)
            .Where(u => !u.IsDeleted && u.UserRoles.Any(ur => ur.Role.Name == AppRoles.Staff))
            .AsQueryable();

        var query = baseQuery
            .GroupJoin(
                context.StaffExtension,
                user => user.Id,
                ext => ext.StaffId,
                (user, extGroup) => new { user, extGroup })
            .SelectMany(
                x => x.extGroup.DefaultIfEmpty(),
                (x, ext) => new { x.user, ext })
            .GroupJoin(
                userManager.Users,
                x => x.ext != null ? x.ext.CreatedBy : null,
                creator => creator.Id,
                (x, creatorGroup) => new { x.user, x.ext, creatorGroup })
            .SelectMany(
                x => x.creatorGroup.DefaultIfEmpty(),
                (x, creator) => new
                {
                    Staff = x.user,
                    Creator = creator
                }
            )
            .Where(x => (x.Staff == null) || (!x.Staff.IsDeleted && (x.Staff.StaffExtension != null)))
            .Select(x => new StaffMappingDto
            {
                Id = x.Staff.Id,
                UserName = x.Staff.UserName,
                UserRoles = x.Staff.UserRoles,
                IsNew = x.Staff.IsNew,
                Status = x.Staff.Status,
                CreatedAt = x.Staff.CreatedAt,
                UpdatedAt = x.Staff.UpdatedAt,
                Admin = x.Creator
            });

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<StaffMappingDto>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<PaginationResult<AppUser>> GetAdminByCriteriaAsync(
        Func<IQueryable<AppUser>, IQueryable<AppUser>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = userManager.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => !u.IsDeleted &&
                        u.UserRoles.Any(ur => ur.Role.NormalizedName == AppRoles.Admin.ToUpper()))
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<AppUser>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }
}
