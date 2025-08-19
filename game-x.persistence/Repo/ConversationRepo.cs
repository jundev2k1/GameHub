using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;
using game_x.share.Helper;
using Mapster;

namespace game_x.persistence.Repo;

public class ConversationRepo(GameXContext context): IConversationRepo, IRepository
{
    // ---- Cursor-based queue for unassigned support conversations (next-only) ----
public async Task<CursorResult<ConversationQueueItemDto>> GetUnassignedQueueByCursorAsync(
    int limit,
    string? cursor,
    string? q,
    string? search,
    CancellationToken ct = default)
{
    // --- Base feed: only open, unassigned support conversations ---
    var query = context.Conversations
        .AsNoTracking()
        .Where(c => c.Type == ConversationType.Support
                 && c.Status == ConversationStatus.Open
                 && c.AssignedAgentId == null);

    // --- Optional search (ILIKE on customer id / username / nickname / message text) ---
    if (!string.IsNullOrWhiteSpace(search))
    {
        var s = $"%{search.Trim()}%";
        query = query.Where(c =>
            (c.CustomerId != null && EF.Functions.ILike(c.CustomerId, s)) ||
            context.Users.Where(u => u.Id == c.CustomerId)
                         .Any(u => EF.Functions.ILike(u.UserName!, s) || EF.Functions.ILike(u.Nickname, s)) ||
            context.Messages.Where(m => m.ConversationId == c.Id)
                            .Any(m => EF.Functions.ILike(m.Text ?? string.Empty, s)));
    }

    // --- Include customer + last message (for DTO mapping & preview) ---
        // Keep only conversations that have messages.
    var src = query
        .Include(c => c.Customer)
        .Include(c => c.Messages
            .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
            .Take(1))
        .Where(c => c.Messages.Any());

    // Helper for preview string (server-safe)
    static string Preview(string? text)
        => string.IsNullOrWhiteSpace(text) ? "[Attachment]"
         : text.Length <= 140 ? text
         : text[..140];

    // --- Build via seek builder: ---
    //    - Key1: LastMessageAt (UTC)
    //    - Key2: LastMessage.Id (tie-breaker; use the same order as above)
    //    - Sort: newest first (DESC, DESC)
    //    - FromCursor: opaque token + optional filter fingerprint (stable across searches)
    var fp = CursorHelper.ComputeFp($"q:{search?.Trim().ToLowerInvariant() ?? string.Empty}");

    return await SeekCursorBuilder<Conversation>
        .For(src)
        .Keys(
            c => c.LastMessageAt,
            c => c.Messages
                   .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
                   .Select(m => m.Id)
                   .FirstOrDefault())
        .Sort(desc1: true, desc2: true) // newest → older
        .FromCursor(cursor, fp) // next-only feed; prev is omitted
        .WithPrev(false)
        .Limit(limit)
        .ExecuteAsync(c =>
        {
            var dto = c.Adapt<ConversationQueueItemDto>();
            return dto with { LastMessagePreview = Preview(dto.LastMessagePreview) };
        }, ct);
}
    
    public async Task<Conversation?> GetSupportConversationAsync(ConversationStatus status, string customerId, CancellationToken ct = default)
    {
        return await context.Conversations
            .AsTracking()
            .FirstOrDefaultAsync(c =>
                c.Type == ConversationType.Support &&
                c.Status == status &&
                c.CustomerId == customerId, ct);
    }
    
    public async Task AddAsync(Conversation conv, CancellationToken ct = default)
    {
        await context.Conversations.AddAsync(conv, ct);
    }
}