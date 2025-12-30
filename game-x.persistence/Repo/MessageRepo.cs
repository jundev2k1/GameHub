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
            .Include(m => m.SenderUser)
                .ThenInclude(m => m!.Avatar)
            .Include(m => m.Mentions)
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
                SenderUser = m.SenderUser,
                Kind = m.Kind,
                Text = m.Text,
                ReplyToMessageId = m.ReplyToMessage!.PublicId,
                IsTombstone = m.IsTombstone,
                SentAt = m.SentAt,
                EditedAt = m.EditedAt,
                EditCount = m.EditCount,
                IsMentionAll = m.IsMentionAll,
                CurrentVersion = m.CurrentVersion,
                DirectMentions =  m.Mentions
                    .Select(x => new DirectMention(x.UserId, x.Display ?? string.Empty))
                    .ToList(),
                Attachments = m.Attachments
                    .Select(a => a.Adapt<MessageAttachmentDto>())
                    .ToList()
            });
            
        return Task.FromResult(query);
    }
    
    // -------- Window/Anchor (jump to message) --------
    public async Task<MessageWindowDto> GetWindowAsync(
        Guid convId, Guid anchorId, int before, int after, WindowAnchorType anchor, CancellationToken ct = default)
    {
        var baseQuery = context.Messages
            .AsNoTracking()
            .Include(m => m.ReplyToMessage)
            .Include(m => m.SenderUser)
            .Include(x => x.Conversation)
            .AsQueryable();
        
        // 1) Resolve anchor pivot (self or reply target)
        var meta = await baseQuery
                .Where(m => m.Conversation.PublicId == convId && m.PublicId == anchorId)
                .Select(m => new { m.Id, m.PublicId, m.SentAt, m.ReplyToMessageId })
                .SingleOrDefaultAsync(ct)
            ?? throw new NotFoundException(MessageCode.Chatting.MessageNotFound);

        var pivotId = meta.Id;
        var pivotAt = meta.SentAt;

        if (anchor.Equals(WindowAnchorType.ReplyToTarget) && meta.ReplyToMessageId != null)
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
            .Select(m =>
            {
                var msgDto = m.Adapt<MessageDto>();
                return msgDto.Adapt<ListedMessageDto>();
            })
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
    
    public async Task<Message?> CheckExistByConvIdAsync(Guid convId, Guid msgId, CancellationToken ct = default)
    {
        var convPk = await context.Conversations
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(c => c.PublicId == convId)
            .Select(c => c.Id)
            .SingleOrDefaultAsync(ct);
        
        return await context.Messages.AsNoTracking()
            .Where(m => m.ConversationId == convPk && m.PublicId == msgId)
            .FirstOrDefaultAsync(ct);

    }
    
    public async Task<Message> CheckExistAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Messages.AsNoTracking()
            .Include(x => x.Conversation)
            .Where(m => m.PublicId == id)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(MessageCode.Chatting.MessageNotFound);
    }
    
    public async Task<Message> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Messages.AsNoTracking()
            .Include(x => x.Conversation)
            .Include(x => x.SenderUser)
                .ThenInclude(x => x!.Avatar)
            .Include(x => x.Attachments)
                .ThenInclude(x => x!.MediaFile)
            .Include(x => x.Mentions)
            .Include(x => x.ReplyToMessage)
            .Where(m => m.PublicId == id)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(MessageCode.Chatting.MessageNotFound);

    }
    
    public async Task<Message?> GetLastedMessageAsync(int convId, CancellationToken ct = default)
    {
        return await context.Messages.AsNoTracking()
            .Where(m => m.ConversationId == convId)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync(ct);
    }
    
    public async Task UpdateAsync(Guid id, Action<Message> updateAction, CancellationToken ct = default)
    {
        var convMember = await context.Messages
             .FirstOrDefaultAsync(c => c.PublicId == id, ct)
         ?? throw new NotFoundException(MessageCode.Chatting.MessageNotFound);
        updateAction.Invoke(convMember);
        await context.SaveChangesAsync(ct);
    }
}