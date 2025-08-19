using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public class ConversationMemberRepo(GameXContext context): IConversationMemberRepo, IRepository
{
    public async Task AddAsync(ConversationMember convMember, CancellationToken ct = default)
    {
        await context.ConversationMembers.AddAsync(convMember, ct);
    }
    
    public async Task<bool> CheckExistMemberAsync(int conversationId, string userId, CancellationToken ct = default)
    {
        return await context.ConversationMembers
            .AsNoTracking()
            .AnyAsync(m => m.ConversationId == conversationId && m.UserId == userId, ct);
    }
}