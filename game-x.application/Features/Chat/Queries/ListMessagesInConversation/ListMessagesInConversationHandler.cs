using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Chat.Queries.ListMessagesInConversation;

public sealed class ListMessagesInConversationHandler(IMessageRepo messageRepo)
    : IRequestHandler<ListMessagesInConversationQuery, CursorResult<MessageDto>>
{
    public async Task<CursorResult<MessageDto>> Handle(ListMessagesInConversationQuery request, CancellationToken ct)
    {
        return await messageRepo.GetByCursorAsync(
            convId: request.ConvId,
            limit: request.Limit ?? 20,
            cursor: request.Cursor,
            ct: ct);
    }
}