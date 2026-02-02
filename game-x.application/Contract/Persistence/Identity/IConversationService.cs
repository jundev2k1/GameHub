using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Persistence.Identity;

public interface IConversationService
{
    Task<Guid> EnsureForPublic(CancellationToken ct);
    Task<Conversation> EnsureForPair(string me, string targetedUserId, CancellationToken ct);
    Task<Guid> EnsureForSupport(string actorId, string? userId, CancellationToken ct);
    Task<CursorResult<SupportConversationDto>> GetUnassignedQueueByCursorAsync(int limit, string? cursor, CancellationToken ct);
    Task<CursorResult<SupportConversationDto>> GetSupportConversationsAsync(int limit, string? cursor, CancellationToken ct);
    Task<CursorResult<ListedConversationDto>> GetHiddenConversationsForClientAsync(string userId, int limit, string? cursor, CancellationToken ct = default);
    Task<CursorResult<ListedConversationDto>> GetMyConversationsForClientAsync(
        string userId,
        int limit,
        string? cursor,
        ConversationType? type,
        CancellationToken ct = default);
    Task<ConversationDetailDto?> GetMyConversationsForGuestAsync(string guestId, CancellationToken ct = default);
    Task<ConversationDetailDto> GetConvByIdAsync(Guid convId, CancellationToken ct = default);
    Task<ConversationDetailDto> GetConvByIdAndUserIdAsync(Guid convId, string userId, CancellationToken ct = default);
}