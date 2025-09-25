using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface ILiveStreamChatRepo
{
    Task<PaginationResult<LiveStreamChatMessage>> GetsByCriteriaAsync(
        Func<IQueryable<LiveStreamChatMessage>, IQueryable<LiveStreamChatMessage>>? builder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<LiveStreamChatMessage> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task CreateAsync(LiveStreamChatMessage message, CancellationToken ct = default);

    Task DeleteAsync(Guid messageId, CancellationToken ct = default);
}
