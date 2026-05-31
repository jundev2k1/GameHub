using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMessageInPublicConversation;

public sealed class ListMessageInPublicConversationHandler(
    IConversationRepo convRepo,
    IMessageService messageService)
    : IQueryHandler<ListMessageInPublicConversationQuery, CursorResult<ListedMessageDto>>
{
    public async Task<CursorResult<ListedMessageDto>> Handle(ListMessageInPublicConversationQuery request, CancellationToken ct = default)
    {
        var publicConv = await convRepo.GetPublicConvAsync(ct);
        return await messageService.GetByCursorAsync(
            convId: publicConv.PublicId,
            limit: request.Limit ?? 20,
            cursor: request.Cursor,
            ct: ct);
    }
}