using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListWindowMessagesInConversation;

public sealed class ListWindowMessagesInConversationHandler(IMessageRepo messageRepo)
    : IRequestHandler<ListWindowMessagesInConversationQuery, MessageWindowDto>
{
    public async Task<MessageWindowDto> Handle(ListWindowMessagesInConversationQuery request, CancellationToken ct)
    {
        return await messageRepo.GetWindowAsync(
            convId: request.ConvId,
            anchorId: request.AnchorId,
            anchor: request.Anchor ?? WindowAnchorType.Self,
            before: request.Before ?? 10,
            after: request.After ?? 10,
            ct: ct);
    }
}