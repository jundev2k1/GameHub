using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface IUserRepo
{
    Task<User[]> GetUserByRole(string roleName, CancellationToken ct = default);

    Task<User> GetUserByIdAsync(string userId, CancellationToken ct = default);

    Task<User> GetUserByEmailAsync(string email, CancellationToken ct = default);

    Task<User[]> GetAdminUsers(CancellationToken ct = default);

    Task<User> GetAdminById(string userId, CancellationToken ct = default);

    Task<UserDetailDto> GetUserDetailAsync(string userId, CancellationToken ct = default);

    Task<UserExtend> GetUserExtendAsync(string userId, CancellationToken ct = default);

    Task<UserKyc> GetKycProfileAsync(string userId, CancellationToken ct = default);

    Task<(KycStatus Status, string? RejectionReason)> GetKycStatusAsync(string userId, CancellationToken ct = default);

    Task<VerificationStatusDto[]> GetVerificationStatusList(string userId, CancellationToken ct = default);

    Task<PaginationResult<UserDto>> GetUserByCriteriaAsync(
    Func<IQueryable<UserDto>, IQueryable<UserDto>>? queryBuilder = null,
    int page = 1,
    int pageSize = 20,
    CancellationToken ct = default);

    Task<bool> IsExistEmailAsync(string email, CancellationToken ct = default);

    Task<bool> IsExistPhoneNumberAsync(string phoneNumber, CancellationToken ct = default);

    Task<bool> IsExistNicknameAsync(string nickname, CancellationToken ct = default);

    Task AddUserAsync(User user, string rawPassword, AppRole role, CancellationToken ct = default);

    Task UpdateAsync(string userId, Action<User> updateAction, CancellationToken ct = default);

    Task UpdateByEmailAsync(string email, Action<User> updateAction, CancellationToken ct = default);

    Task UpdateKycAsync(string userId, Action<UserKyc> updateAction, CancellationToken ct = default);
}
