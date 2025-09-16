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
    Task<PaginationResult<SocialLinkDto>> GetIncomingRequestsByCriteriaAsync(
        string addresseeUserId,
        Func<IQueryable<SocialLinkDto>, IQueryable<SocialLinkDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<PaginationResult<SocialLinkDto>> GetOutgoingRequestsByCriteriaAsync(
        string requesterUserId,
        Func<IQueryable<SocialLinkDto>, IQueryable<SocialLinkDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<SocialLink?> GetByKeyPairAsync(string min, string max, SocialLinkKind kind, CancellationToken ct = default);
    Task<SocialLink> GetByIdAsync(Guid linkId, SocialLinkKind kind, CancellationToken ct = default);
    Task AddAsync(SocialLink link, CancellationToken ct = default);
    Task<SocialLink> UpdateAsync(Guid publicId, Action<SocialLink> updateAction, CancellationToken ct = default);
}