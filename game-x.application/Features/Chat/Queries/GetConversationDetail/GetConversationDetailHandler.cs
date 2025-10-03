using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.GetConversationDetail;

public sealed class GetConversationDetailHandler(IUserAccessor userAccessor, IConversationService conversationService)
    : IRequestHandler<GetConversationDetailQuery, ConversationDetailDto>
{
    public async Task<ConversationDetailDto> Handle(GetConversationDetailQuery request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        return await conversationService.GetConvByIdAndUserIdAsync(request.ConvId, userId, ct);
    }
}