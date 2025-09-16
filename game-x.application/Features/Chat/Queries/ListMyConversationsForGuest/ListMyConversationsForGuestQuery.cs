using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListMyConversationsForGuest;

public record ListMyConversationsForGuestQuery(string GuestId) : IQuery<ListedConversationDto>;