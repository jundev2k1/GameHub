using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Persistence.Identity;

public interface IConversationService
{
    Task<CursorResult<SupportConversationDto>> GetByCursorAsync(Guid convId, int limit, string? cursor, CancellationToken ct);
}