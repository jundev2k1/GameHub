namespace game_x.application.Contract.Persistence.Repo;

public interface IConversationRepo
{
    IQueryable<Conversation> GetUnassignedQueueByCursorAsync(CancellationToken ct = default); 
    IQueryable<Conversation> GetSupportConversationsAsync(CancellationToken ct = default);
    IQueryable<Conversation> GetMyConversationsForClientAsync(string userId, CancellationToken ct = default);
    Task<Conversation?> GetSupportConversationAsync(string actorId, CancellationToken ct = default);
    Task<Conversation> GetConversationDetailAsync(Guid convId, CancellationToken ct = default);
    Task<Conversation?> GetMyConversationsForGuestAsync(string guestId, CancellationToken ct = default);
    Task<Conversation?> FindForPairAsync(string userA, string userB, CancellationToken ct = default);
    Task<Conversation> GetByIdAsync(Guid convId, CancellationToken ct = default);
    Task<Conversation> GetByIdAndActorIdAsync(string actorId, Guid convId, CancellationToken ct = default);
    Task AddAsync(Conversation conv, CancellationToken ct = default);
    Task PatchUpdateAsync(Guid publicId, Action<Conversation> updateAction, CancellationToken ct = default);
}