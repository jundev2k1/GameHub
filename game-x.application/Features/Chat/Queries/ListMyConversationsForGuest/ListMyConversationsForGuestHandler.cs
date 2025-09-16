using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMyConversationsForGuest;

public sealed class ListMyConversationsForGuestHandler(IConversationRepo conversationRepo)
    : IRequestHandler<ListMyConversationsForGuestQuery, ListedConversationDto?>
{
    public async Task<ListedConversationDto?> Handle(ListMyConversationsForGuestQuery request, CancellationToken ct)
    {
        return await conversationRepo.GetMyConversationsForGuestAsync(guestId: request.GuestId, ct: ct);
    }
}