using System.Linq.Expressions;
using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Chat.Dtos;
using game_x.domain.Constants;
using game_x.share.Helper;
using Mapster;

namespace game_x.persistence.Repo;

public class ConversationRepo(GameXContext context): IConversationRepo, IRepository
{
    /// <summary>Short preview helper</summary>
    private string Preview(string? text)
        => string.IsNullOrWhiteSpace(text) ? "[Attachment]"
            : text.Length <= 140 ? text
            : text[..140];
    
    // ---- Cursor-based queue for unassigned support conversations (next-only) ----
    public async Task<CursorResult<SupportConversationDto>> GetUnassignedQueueByCursorAsync(
        int limit,
        string? cursor,
        CancellationToken ct = default)
    {
        // --- Base feed: only open, unassigned support conversations ---
        var query = context.Conversations
            .AsNoTracking()
            .Where(c => c.Type == ConversationType.Support
                     && c.Status == ConversationStatus.Open
                     && c.AssignedAgentId == null);
        
        var src = query
            .Include(c => c.Customer)
            .Include(c => c.Messages
                .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
                .Take(1))
                .ThenInclude(x => x.SenderUser)
            .Where(c => c.Messages.Any());
        
        var fp = CursorHelper.ComputeFp($"q:");

        return await SeekCursorBuilder<Conversation>
            .For(src)
            .Keys(
                c => c.LastMessageAt,
                c => c.Messages
                   .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
                   .Select(m => m.Id)
                   .FirstOrDefault())
            .Sort(desc1: true, desc2: true)
            .FromCursor(cursor, fp)
            .Limit(limit)
            .ExecuteAsync(c =>
            {
                var dto = c.Adapt<SupportConversationDto>();
                return dto with { LastMessagePreview = Preview(dto.LastMessagePreview) };
            }, ct);
        }

    public async Task<CursorResult<SupportConversationDto>> GetSupportConversationsAsync(
        int limit,
        string? cursor,
        CancellationToken ct = default)
    {
        IQueryable<Conversation> query = context.Conversations.AsNoTracking();

        query = query.Where(c => c.Type == ConversationType.Support && c.Status == ConversationStatus.Claimed);

        var src = query
            .Include(c => c.Customer)
            .Include(c => c.Messages
                .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
                .Take(1))
                .ThenInclude(x => x.SenderUser)
            .Where(c => c.Messages.Any());

        // Fingerprint ties the cursor to current user + search
        var fp = CursorHelper.ComputeFp($"|q:");

        return await SeekCursorBuilder<Conversation>
            .For(src)
            .Keys(
                c => c.LastMessageAt,
                c => context.Messages
                 .Where(m => m.ConversationId == c.Id)
                 .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
                 .Select(m => m.Id)
                 .FirstOrDefault())
            .Sort(desc1: true, desc2: true)
            .FromCursor(cursor, fp)
            .Limit(limit)
            .ExecuteAsync(c =>
            {
                var dto = c.Adapt<SupportConversationDto>();
                return dto with { LastMessagePreview = Preview(dto.LastMessagePreview) };
            }, ct);
    }

    public async Task<CursorResult<ConversationDto>> GetMyConversationsForClientAsync(
        string userId,
        int limit,
        string? cursor,
        CancellationToken ct = default)
    {
        IQueryable<Conversation> query = context.Conversations.AsNoTracking();

        // Retrieve all private conversations and group chats
        Expression<Func<Conversation, bool>> minePredicate =
            c => c.CustomerId == userId
              || context.ConversationMembers.Any(m => m.ConversationId == c.Id && m.UserId == userId);

        query = query.Where(minePredicate);

        var src = query
            .Include(c => c.Customer)
            .Include(c => c.Messages
                .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
                .Take(1))
                .ThenInclude(x => x.SenderUser)
            .Where(c => c.Messages.Any());
        
        var fp = CursorHelper.ComputeFp($"me:{userId}|q:");
        
        return await SeekCursorBuilder<Conversation>
            .For(src)
            .Keys(
                c => c.LastMessageAt,
                c => context.Messages
                 .Where(m => m.ConversationId == c.Id)
                 .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
                 .Select(m => m.Id)
                 .FirstOrDefault())
            .Sort(desc1: true, desc2: true)
            .FromCursor(cursor, fp)
            .Limit(limit)
            .ExecuteAsync(c =>
            {
                var dto = c.Adapt<ConversationDto>();
                return dto with { LastMessagePreview = Preview(dto.LastMessagePreview) };
            }, ct);
    }

    public async Task<ConversationDto?> GetMyConversationsForGuestAsync(string guestId, CancellationToken ct = default)
    {
        IQueryable<Conversation> query = context.Conversations.AsNoTracking();

        // Retrieve all private conversations and group chats
        Expression<Func<Conversation, bool>> minePredicate = c => c.GuestId == guestId;

        query = query.Where(minePredicate);

        var src = await query
            .AsTracking()
            .OrderDescending()
            .Include(c => c.Messages
                .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
                .Take(1))
            .Where(c => c.Messages.Any())
            .FirstOrDefaultAsync(c =>
                c.Type == ConversationType.Support &&
                c.GuestId == guestId, ct);
        
        if (src == null)
            return null;
        
        var dto = src.Adapt<ConversationDto>();
        return dto with { LastMessagePreview = Preview(dto.LastMessagePreview) };
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