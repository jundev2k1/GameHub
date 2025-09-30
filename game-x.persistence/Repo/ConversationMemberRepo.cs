using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Chat.Dtos;
using game_x.domain.Constants;

namespace game_x.persistence.Repo;

public class ConversationMemberRepo(GameXContext context): IConversationMemberRepo, IRepository
{
    public async Task<List<ConvUnreadDto>> GetAllUnReads(string userId, CancellationToken ct = default)
    {
        return await context.ConversationMembers
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.IsHidden != true)
            .Select(m => new
            {
                m.Conversation.PublicId,
                Unread = context.Messages.Count(x =>
                x.ConversationId == m.ConversationId 
                && (m.LastReadMessageId == null || x.Id > m.LastReadMessageId)),
            })
            .Where(x => x.Unread > 0)
            .Select(x => new ConvUnreadDto(x.PublicId, x.Unread))
            .ToListAsync(ct);
    }
    
    public async Task<ConvUnreadDto?> GetUnreadAsync(int convId, string userId, CancellationToken ct = default)
    {
        return await context.ConversationMembers
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.ConversationId == convId)
            .Select(m => new ConvUnreadDto (
                m.Conversation.PublicId,
         context.Messages.Count(x =>
                  x.ConversationId == m.ConversationId 
                    && (m.LastReadMessageId == null || x.Id > m.LastReadMessageId))))
            .FirstOrDefaultAsync(ct);
    }
    
    public async Task<bool> CheckExistMemberAsync(int convId, string userId, CancellationToken ct = default)
    {
        return await context.ConversationMembers
            .AsNoTracking()
            .AnyAsync(m => m.ConversationId == convId && m.UserId == userId, ct);
    }
    
    public async Task<ConversationMember?> GetByConvIdAndUserIdAsync(Guid convId, string userId, CancellationToken ct = default)
    {
        return await context.ConversationMembers
            .AsNoTracking()
            .Include(x => x.Conversation)
            .FirstOrDefaultAsync(m => m.Conversation.PublicId == convId && m.UserId == userId, ct);
    }
    
    public async Task<ConvMemberDto[]> GetMembersByConvIdAsync(Guid convId, CancellationToken ct = default)
    {
        if (convId == Guid.Empty) return [];
        
        var convPk = await context.Conversations
            .AsNoTracking()
            .Where(c => c.PublicId == convId)
            .Select(c => c.Id)
            .SingleOrDefaultAsync(ct);
        
        if (EqualityComparer<object>.Default.Equals(convPk, null)) return [];
        
        return await context.ConversationMembers
            .AsNoTracking()
            .Where(m => m.ConversationId.Equals(convPk))
            .Select(m => new ConvMemberDto
            {
                UserId = m.UserId, 
                IsHidden = m.IsHidden
            })
            .ToArrayAsync(ct);
    }
    
    public async Task AddAsync(ConversationMember convMember, CancellationToken ct = default)
    {
        await context.ConversationMembers.AddAsync(convMember, ct);
    }
    
    public async Task UpdateAsync(int id, Action<ConversationMember> updateAction, CancellationToken ct = default)
    {
        var convMember = await context.ConversationMembers
                       .FirstOrDefaultAsync(c => c.Id == id, ct)
                   ?? throw new NotFoundException(MessageCode.Chatting.ConversationMemberNotFound);

        updateAction.Invoke(convMember);
        await context.SaveChangesAsync(ct);
    }
}