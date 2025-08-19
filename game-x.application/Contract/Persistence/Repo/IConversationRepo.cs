using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface IConversationRepo
{
    Task<CursorResult<ConversationQueueItemDto>> GetUnassignedQueueByCursorAsync(
        int limit,
        string? cursor,
        string? q,
        string? search,
        CancellationToken ct = default);
    Task<Conversation?> GetSupportConversationAsync(ConversationStatus status, string customerId, CancellationToken ct = default);
    Task AddAsync(Conversation conv, CancellationToken ct = default);
}