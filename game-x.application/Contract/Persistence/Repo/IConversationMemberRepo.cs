using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface IConversationMemberRepo
{
    Task<List<ConvUnreadDto>> GetAllUnReads(string userId, CancellationToken ct = default);
    Task<ConvUnreadDto?> GetUnreadAsync(int convId, string userId, CancellationToken ct = default);
    Task<bool> CheckExistMemberAsync(int convId, string userId, CancellationToken ct = default);
    Task<ConvMemberDto[]> GetMembersByConvIdAsync(Guid convId, CancellationToken ct = default);
    Task<ConversationMember?> GetByConvIdAndUserIdAsync(Guid convId, string userId, CancellationToken ct = default);
    Task AddAsync(ConversationMember conv, CancellationToken ct = default);
    Task UpdateAsync(int id, Action<ConversationMember> updateAction, CancellationToken ct = default);
}