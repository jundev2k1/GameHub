using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMyConversationsForClient;

public sealed class ListMyConversationsHandler(IUserAccessor userAccessor, IConversationRepo conversationRepo)
    : IRequestHandler<ListMyConversationsForClientQuery, CursorResult<ConversationDto>>
{
    public async Task<CursorResult<ConversationDto>> Handle(ListMyConversationsForClientQuery request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();

        return await conversationRepo.GetMyConversationsForClientAsync(
            userId: userId,
            limit: request.Limit ?? 20,
            cursor: request.Cursor,
            ct: ct);
    }
}