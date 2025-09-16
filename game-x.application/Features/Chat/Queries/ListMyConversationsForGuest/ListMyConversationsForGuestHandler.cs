using game_x.application.Contract.Persistence.Identity;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMyConversationsForGuest;

public sealed class ListMyConversationsForGuestHandler(IConversationService conversationService)
    : IRequestHandler<ListMyConversationsForGuestQuery, ListedConversationDto?>
{
    public async Task<ListedConversationDto?> Handle(ListMyConversationsForGuestQuery request, CancellationToken ct)
    {
        return await conversationService.GetMyConversationsForGuestAsync(guestId: request.GuestId, ct: ct);
    }
}