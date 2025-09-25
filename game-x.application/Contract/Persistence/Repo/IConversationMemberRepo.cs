namespace game_x.application.Contract.Persistence.Repo;

public interface IConversationMemberRepo
{
    Task AddAsync(ConversationMember conv, CancellationToken ct = default);
    Task<bool> CheckExistMemberAsync(int convId, string userId, CancellationToken ct = default);
    Task<string[]> GetMemberIdsAsync(Guid convId, CancellationToken ct = default);
    Task<ConversationMember?> GetByConvIdAndUserIdAsync(Guid convId, string userId, CancellationToken ct = default);
    Task UpdateAsync(int id, Action<ConversationMember> updateAction, CancellationToken ct = default);
}