using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface IConversationRepo
{
    Task<CursorResult<SupportConversationDto>> GetUnassignedQueueByCursorAsync(
        int limit,
        string? cursor,
        CancellationToken ct = default); 
    
    Task<CursorResult<SupportConversationDto>> GetSupportConversationsAsync(
        int limit,
        string? cursor,
        CancellationToken ct = default);
    
    Task<CursorResult<ConversationDto>> GetMyConversationsForClientAsync(
        string userId,
        int limit,
        string? cursor,
        CancellationToken ct = default);
    Task<Conversation> GetByIdAsync(Guid convId, CancellationToken ct = default);
    Task<Conversation?> GetSupportConversationAsync(ConversationStatus status, string customerId, CancellationToken ct = default);
    Task AddAsync(Conversation conv, CancellationToken ct = default);

    Task PatchUpdateAsync(Guid publicId, Action<Conversation> updateAction, CancellationToken ct = default);
}