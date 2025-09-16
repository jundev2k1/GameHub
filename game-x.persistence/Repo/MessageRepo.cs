using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Chat.Dtos;
using game_x.domain.Constants;
using game_x.share.Helper;
using Mapster;

namespace game_x.persistence.Repo;

public class MessageRepo(GameXContext context): IMessageRepo, IRepository
{
    public async Task AddAsync(Message msg, CancellationToken ct = default)
    {
        await context.Messages.AddAsync(msg, ct);
    }
    
    public Task<IQueryable<MessageDto>> GetByCursorAsync(
        Guid convId, int limit, string? cursor, CancellationToken ct = default)
    {
        // Index lookup
        var convDbId = context.Conversations
            .Where(c => c.PublicId == convId)
            .Select(c => c.Id)
            .FirstOrDefault();
        
        var query = context.Messages
            .AsNoTracking()
            .Include(m => m.ReplyToMessage)
            .Include(m => m.Attachments)
                .ThenInclude(a => a.MediaFile)
            .Where(m => m.ConversationId == convDbId)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                PublicId = m.PublicId,
                ConversationId = convId,
                SenderActorId = m.SenderActorId,
                SenderRole = m.SenderRole,
                Kind = m.Kind,
                Text = m.Text,
                ReplyToMessageId = m.ReplyToMessage!.PublicId,
                IsTombstone = m.IsTombstone,
                SentAt = m.SentAt,
                EditedAt = m.EditedAt,
                EditCount = m.EditCount,
                CurrentVersion = m.CurrentVersion,
                Attachments = m.Attachments
                    .Select(a => new MessageAttachmentDto
                    {
                        SortOrder = a.SortOrder,
                        BindingStatus = a.BindingStatus,
                        FileName = a.MediaFile != null ? a.MediaFile.FileName : null,
                        BucketName = a.MediaFile != null ? a.MediaFile.BucketName : null,
                        ObjectName = a.MediaFile != null ? a.MediaFile.ObjectName : null,
                    })
                    .ToList()
            });
            
        return Task.FromResult(query);
    }
    
    // -------- Window/Anchor (jump to message) --------
    public async Task<MessageWindowDto> GetWindowAsync(
        Guid convId, Guid anchorId, int before, int after, string anchor = "self", CancellationToken ct = default)
    {
        var baseQuery = context.Messages.AsNoTracking()
            .Include(x => x.Conversation)
            .AsQueryable();
        
        // 1) Resolve anchor pivot (self or reply target)
        var meta = await baseQuery
            .Where(m => m.Conversation.PublicId == convId && m.PublicId == anchorId)
            .Select(m => new { m.Id, m.SentAt, m.ReplyToMessageId })
            .SingleOrDefaultAsync(ct);

        if (meta is null) throw new KeyNotFoundException("Message not found");

        var pivotId = meta.Id;
        var pivotAt = meta.SentAt;

        if (anchor.Equals("replyTarget", StringComparison.OrdinalIgnoreCase) && meta.ReplyToMessageId != null)
        {
            var target = await baseQuery
                .Where(m => m.Conversation.PublicId == convId && m.Id == meta.ReplyToMessageId.Value)
                .Select(m => new { m.Id, m.SentAt })
                .SingleOrDefaultAsync(ct);

            if (target is not null)
            {
                pivotId = target.Id;
                pivotAt = target.SentAt;
            }
        }

        // 2) Older side (DESC in DB, then reverse to ASC)
        var older = await baseQuery
            .Where(m => m.Conversation.PublicId == convId &&
                        (m.SentAt < pivotAt || (m.SentAt == pivotAt && m.Id < pivotId)))
            .OrderByDescending(m => m.SentAt).ThenByDescending(m => m.Id)
            .Take(before)
            .ToListAsync(ct);
        older.Reverse();

        // 3) Anchor (full)
        var anchorMsg = await baseQuery
            .FirstAsync(m => m.Conversation.PublicId == convId && m.Id == pivotId, ct);

        // 4) Newer side (ASC)
        var newer = await baseQuery
            .Where(m => m.Conversation.PublicId == convId &&
                        (m.SentAt > pivotAt || (m.SentAt == pivotAt && m.Id > pivotId)))
            .OrderBy(m => m.SentAt).ThenBy(m => m.Id)
            .Take(after)
            .ToListAsync(ct);

        // 5) Build window
        var items = older.Concat([anchorMsg]).Concat(newer)
            .Select(m => m.Adapt<ListedMessageDto>())
            .ToList();

        var fp = CursorHelper.ComputeFp($"conv:{convId}");

        // prev = go newer from first kept; next = go older from last kept
        string? prev = older.Any()
            ? CursorHelper.Token.EncodeKeys(older.First().SentAt, older.First().Id, CursorHelper.Dir.Newer, "k2-1", fp)
            : null;

        string? next = newer.Any()
            ? CursorHelper.Token.EncodeKeys(newer.Last().SentAt, newer.Last().Id, CursorHelper.Dir.Older, "k2-1", fp)
            : null;

        return new MessageWindowDto
        {
            Items = items,
            AnchorMessageId = pivotId,
            HasMoreBefore = older.Count == before,
            HasMoreAfter = newer.Count == after,
            PrevCursor = prev,
            NextCursor = next
        };
    }
    
    public async Task<Message> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Messages.AsNoTracking()
            .Include(x => x.Conversation)
            .Where(m => m.PublicId == id)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(MessageCode.Chatting.MessageNotFound);

    }
    
    // --- Helpers ---

    // Simple snippet around the first match (case-insensitive).
    private static string BuildSnippet(string text, string term, int ctxChars = 30, int maxLen = 140)
    {
        if (string.IsNullOrWhiteSpace(text)) return "[Attachment]";
        var idx = text.IndexOf(term, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return text.Length <= maxLen ? text : text[..maxLen];

        var start = Math.Max(0, idx - ctxChars);
        var end = Math.Min(text.Length, idx + term.Length + ctxChars);
        var snippet = text[start..end];
        if (start > 0) snippet = "…" + snippet;
        if (end < text.Length) snippet += "…";
        return snippet.Length <= maxLen ? snippet : snippet[..maxLen];
    }
}