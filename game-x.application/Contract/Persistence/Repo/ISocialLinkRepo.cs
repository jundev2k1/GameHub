using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Friends.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface ISocialLinkRepo
{
    Task<PaginationResult<FriendDto>> GetFriendshipsAsync(
        string userId,
        Func<IQueryable<FriendDto>, IQueryable<FriendDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<PaginationResult<FriendSearchDto>> SearchFriendshipsAsync(
        string userId,
        Func<IQueryable<FriendSearchDto>, IQueryable<FriendSearchDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<PaginationResult<SocialLink>> GetIncomingRequestsByCriteriaAsync(
        string addresseeUserId,
        Func<IQueryable<SocialLink>, IQueryable<SocialLink>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<PaginationResult<SocialLink>> GetOutgoingRequestsByCriteriaAsync(
        string requesterUserId,
        Func<IQueryable<SocialLink>, IQueryable<SocialLink>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<PaginationResult<SocialLink>> GetBlockedFriendsAsync(
        string userId,
        Func<IQueryable<SocialLink>, IQueryable<SocialLink>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<SocialLink?> GetByKeyPairAsync(string min, string max, CancellationToken ct = default);
    Task<SocialLink> GetByIdAsync(Guid linkId, CancellationToken ct = default);
    Task AddAsync(SocialLink link, CancellationToken ct = default);
    Task<SocialLink> UpdateAsync(Guid publicId, Action<SocialLink> updateAction, CancellationToken ct = default);
    
    /// <summary>Get entity and lock row; it prevents race condition, using in transaction.</summary>
    Task<SocialLink> GetForUpdateAsync(Guid publicId, CancellationToken ct = default);
}