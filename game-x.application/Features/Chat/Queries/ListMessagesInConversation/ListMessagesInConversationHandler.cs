using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMessagesInConversation;

public sealed class ListMessagesInConversationHandler(
    IConversationRepo conversationRepo,
    IMessageService messageService)
    : IRequestHandler<ListMessagesInConversationQuery, CursorResult<ListedMessageDto>>
{
    public async Task<CursorResult<ListedMessageDto>> Handle(ListMessagesInConversationQuery request, CancellationToken ct)
    {
        // Require permission for users and guests, optional for CS and admins
        if(request.ActorId is not null)
            await conversationRepo.GetByIdAndActorIdAsync(request.ActorId, request.ConvId, ct);
        
        return await messageService.GetByCursorAsync(
            convId: request.ConvId,
            limit: request.Limit ?? 20,
            cursor: request.Cursor,
            ct: ct);
    }
}