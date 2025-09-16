using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Persistence.Identity;

public interface IConversationService
{
    Task<CursorResult<SupportConversationDto>> GetUnassignedQueueByCursorAsync(int limit, string? cursor, CancellationToken ct);
    Task<CursorResult<SupportConversationDto>> GetSupportConversationsAsync(int limit, string? cursor, CancellationToken ct);
    Task<CursorResult<ListedConversationDto>> GetMyConversationsForClientAsync(string userId, int limit, string? cursor, CancellationToken ct = default);
    Task<ListedConversationDto?> GetMyConversationsForGuestAsync(string guestId, CancellationToken ct = default);
    Task<ConversationDetailDto> GetConversationDetailAsync(Guid convId, CancellationToken ct = default);
}