using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Chat.Queries.ListWindowMessagesInConversation;

public sealed class ListWindowMessagesInConversationHandler(IMessageRepo messageRepo)
    : IRequestHandler<ListWindowMessagesInConversationQuery, MessageWindowDto>
{
    public async Task<MessageWindowDto> Handle(ListWindowMessagesInConversationQuery request, CancellationToken ct)
    {
        return await messageRepo.GetWindowAsync(
            convId: request.ConvId,
            anchorId: request.AnchorId,
            anchor: request.Anchor,
            before: request.Before,
            after: request.After,
            ct: ct);
    }
}