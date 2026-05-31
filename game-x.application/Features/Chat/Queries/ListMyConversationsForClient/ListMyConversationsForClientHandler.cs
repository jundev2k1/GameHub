using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMyConversationsForClient;

public sealed class ListMyConversationsForClientHandler(IUserAccessor userAccessor, IConversationService conversationService)
    : IRequestHandler<ListMyConversationsForClientQuery, CursorResult<ListedConversationDto>>
{
    public async Task<CursorResult<ListedConversationDto>> Handle(ListMyConversationsForClientQuery request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        return await conversationService.GetMyConversationsForClientAsync(
            userId: userId,
            limit: request.Limit ?? 20,
            cursor: request.Cursor,
            type: request.Type,
            ct: ct);
    }
}