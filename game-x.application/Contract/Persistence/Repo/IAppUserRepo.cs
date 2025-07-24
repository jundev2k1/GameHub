using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.AccountManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Contract.Persistence.Repo;

public interface IAppUserRepo
{
    Task<PaginationResult<UserMappingDto>> GetUserByCriteriaAsync(
        Func<IQueryable<UserMappingDto>, IQueryable<UserMappingDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<PaginationResult<StaffMappingDto>> GetStaffByCriteriaAsync(
        Func<IQueryable<StaffMappingDto>, IQueryable<StaffMappingDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<PaginationResult<AppUser>> GetAdminByCriteriaAsync(
        Func<IQueryable<AppUser>, IQueryable<AppUser>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<AppUser?> GetUserDetailByIdAsync(string userId, CancellationToken ct = default);

    Task<AppUser?> GetStaffDetailByIdAsync(string staffId, CancellationToken ct = default);

    Task<AppUser?> GetAdminByIdAsync(string adminId, CancellationToken ct = default);
}