using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface ILiveStreamDonationRepo
{
    Task<PaginationResult<LiveStreamDonation>> GetsByCriteriaAsync(
        Func<IQueryable<LiveStreamDonation>, IQueryable<LiveStreamDonation>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<LiveStreamDonation> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task CreateAsync(LiveStreamDonation donation, CancellationToken ct = default);
}
