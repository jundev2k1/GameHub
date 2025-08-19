namespace game_x.application.Contract.Persistence.Repo;

public interface IConversationMemberRepo
{
    Task AddAsync(ConversationMember conv, CancellationToken ct = default);
    Task<bool> CheckExistMemberAsync(int conversationId, string userId, CancellationToken ct = default);
}