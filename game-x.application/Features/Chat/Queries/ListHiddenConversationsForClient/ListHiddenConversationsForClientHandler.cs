using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListHiddenConversationsForClient;

public sealed class ListHiddenConversationsForClientHandler(IUserAccessor userAccessor, IConversationService conversationService)
    : IRequestHandler<ListHiddenConversationsForClientQuery, CursorResult<ListedConversationDto>>
{
    public async Task<CursorResult<ListedConversationDto>> Handle(ListHiddenConversationsForClientQuery request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        return await conversationService.GetHiddenConversationsForClientAsync(
            userId: userId,
            limit: request.Limit ?? 20,
            cursor: request.Cursor,
            ct: ct);
    }
}