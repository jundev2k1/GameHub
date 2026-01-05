using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListSupportConvUnread;

public sealed class ListSupportConvUnreadHandler(IConversationRepo convRepo)
    : IRequestHandler<ListSupportConvUnreadQuery, IReadOnlyCollection<ConversationUnreadDto>>
{
    public async Task<IReadOnlyCollection<ConversationUnreadDto>> Handle(ListSupportConvUnreadQuery request, CancellationToken ct)
    {
        return await convRepo.GetSupportConvUnreadAsync(ct);
    }
}