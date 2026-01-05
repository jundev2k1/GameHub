using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface IConversationRepo
{
    IQueryable<ConversationItemDto> GetUnassignedQueueByCursorAsync(string userId, CancellationToken ct = default); 
    IQueryable<ConversationItemDto> GetSupportConversationsAsync(string userId, CancellationToken ct = default);
    IQueryable<ConversationItemDto> GetHiddenConversationsForClientAsync(string userId, CancellationToken ct = default);
    IQueryable<ConversationItemDto> GetMyConversationsForClientAsync(string userId, CancellationToken ct = default);
    Task<Conversation?> GetSupportConversationAsync(string actorId, CancellationToken ct = default);
    Task<IReadOnlyCollection<ConversationUnreadDto>> GetSupportConvUnreadAsync(CancellationToken ct = default);
    Task<int> CountSupportConvUnreadAsync(Guid id, CancellationToken ct = default);
    Task<ConversationItemDto> GetConvByIdAsync(Guid convId, CancellationToken ct = default);
    Task<Conversation> GetPublicConvAsync(CancellationToken ct = default);
    Task<ConversationItemDto> GetConvByIdAndUserIdAsync(Guid convId, string userId, CancellationToken ct = default);
    Task<Conversation?> GetMyConversationsForGuestAsync(string guestId, CancellationToken ct = default);
    Task<Conversation?> FindForPairAsync(string userA, string userB, CancellationToken ct = default);
    Task<Conversation?> FindPublicAsync(CancellationToken ct = default);
    Task<Conversation> GetByIdAsync(Guid convId, CancellationToken ct = default);
    Task<Conversation> GetByIdAndActorIdAsync(string actorId, Guid convId, CancellationToken ct = default);
    Task AddAsync(Conversation conv, CancellationToken ct = default);
    Task UpdateAsync(Guid publicId, Action<Conversation> updateAction, CancellationToken ct = default);
}