using System.Linq.Expressions;
using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.domain.Constants;

namespace game_x.persistence.Repo;

public class ConversationRepo(GameXContext context): IConversationRepo, IRepository
{
    private IQueryable<Conversation> BuildConversationListing(IQueryable<Conversation> query)
    {
        return query
            .AsNoTracking()
            .Include(c => c.Customer)
                .ThenInclude(c => c!.Avatar)
            .Include(c => c.Messages
                .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
                .Take(1))
                .ThenInclude(x => x.SenderUser)
                    .ThenInclude(x => x!.Avatar)
            .Where(c => c.Messages.Any() && c.Type != ConversationType.Public);
    }
    
    // ---- Cursor-based queue for unassigned support conversations (next-only) ----
    public IQueryable<Conversation> GetUnassignedQueueByCursorAsync(CancellationToken ct = default)
    {
        // --- Base feed: only open, unassigned support conversations ---
        var query = context.Conversations
            .Where(c => c.Type == ConversationType.Support
                     && c.Status == ConversationStatus.Open
                     && c.AssignedAgentId == null);
        return BuildConversationListing(query);
    }

    public IQueryable<Conversation> GetSupportConversationsAsync(CancellationToken ct = default)
    {
        IQueryable<Conversation> query = context.Conversations
            .Where(c => c.Type == ConversationType.Support && c.Status == ConversationStatus.Claimed);
        return BuildConversationListing(query);
    }

    public IQueryable<Conversation> GetMyConversationsForClientAsync(string userId, CancellationToken ct = default)
    {
        // Retrieve all private conversations and group chats
        Expression<Func<Conversation, bool>> minePredicate =
            c => c.CustomerId == userId
              || context.ConversationMembers.Any(m => m.ConversationId == c.Id && m.UserId == userId);
        IQueryable<Conversation> query = context.Conversations.Where(minePredicate);
        return BuildConversationListing(query);
    }

    public async Task<Conversation?> GetMyConversationsForGuestAsync(string guestId, CancellationToken ct = default)
    {
        // Retrieve all private conversations and group chats
        Expression<Func<Conversation, bool>> minePredicate = c => c.GuestId == guestId;
        IQueryable<Conversation> query = context.Conversations.Where(minePredicate);
        return await query
            .AsTracking()
            .OrderDescending()
            .Include(c => c.Messages
                .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
                .Take(1))
            .Where(c => c.Messages.Any())
            .FirstOrDefaultAsync(c =>
                c.Type == ConversationType.Support &&
                c.GuestId == guestId, ct);
    }
    
    public async Task<Conversation?> GetSupportConversationAsync(string actorId, CancellationToken ct = default)
    {
        return await context.Conversations
            .AsTracking()
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c =>
                c.Type == ConversationType.Support &&
                c.CustomerId == actorId || c.GuestId == actorId, ct);
    }
    
    public async Task<Conversation> GetConversationDetailAsync(Guid convId, CancellationToken ct = default)
    {
        return await context.Conversations
                       .AsTracking()
                       .Include(c => c.Customer)
                           .ThenInclude(c => c!.Avatar)
                       .Include(c => c.Messages
                           .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
                           .Take(1))
                           .ThenInclude(x => x.SenderUser)
                                .ThenInclude(x => x!.Avatar)
                       .FirstOrDefaultAsync(c => c.PublicId == convId, ct)
                   ?? throw new NotFoundException(MessageCode.Chatting.ConversationNotFound);
    }
    
    public async Task<Conversation?> FindForPairAsync(string userA, string userB, CancellationToken ct = default)
    {
        return await context.Conversations
            .Include(c => c.Members)
            .Where(c => c.Type == ConversationType.Direct)
            .FirstOrDefaultAsync(c =>
                c.Members.Count == 2 &&
                c.Members.Any(m => m.UserId == userA) &&
                c.Members.Any(m => m.UserId == userB), ct);
    }
    
    public async Task<Conversation?> FindPublicAsync(CancellationToken ct = default)
    {
        return await context.Conversations
            .Where(c => c.Type == ConversationType.Public)
            .FirstOrDefaultAsync(ct);
    }
    
    public async Task<Conversation> GetByIdAsync(Guid convId, CancellationToken ct = default)
    {
        return await context.Conversations
            .AsTracking()
            .FirstOrDefaultAsync(c => c.PublicId == convId, ct)
            ?? throw new NotFoundException(MessageCode.Chatting.ConversationNotFound);
    }
    
    public async Task<Conversation> GetByIdAndActorIdAsync(string actorId, Guid convId, CancellationToken ct = default)
    {
        return await context.Conversations
            .AsTracking()
            .FirstOrDefaultAsync(c => 
                c.PublicId == convId
                && (c.CustomerId == actorId
                    || c.GuestId == actorId
                    || context.ConversationMembers.Any(m => m.ConversationId == c.Id && m.UserId == actorId)), 
                ct)
            ?? throw new NotFoundException(MessageCode.Chatting.ConversationNotFound);
    }
    
    public async Task AddAsync(Conversation conv, CancellationToken ct = default)
    {
        await context.Conversations.AddAsync(conv, ct);
    }
    
    public async Task PatchUpdateAsync(Guid publicId, Action<Conversation> updateAction, CancellationToken ct = default)
    {
        var conv = await context.Conversations
            .FirstOrDefaultAsync(c => c.PublicId == publicId, ct)
                ?? throw new NotFoundException(MessageCode.Chatting.ConversationNotFound);

        updateAction.Invoke(conv);
        await context.SaveChangesAsync(ct);
    }
}