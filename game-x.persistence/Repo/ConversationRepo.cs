using System.Linq.Expressions;
using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Chat.Dtos;
using game_x.domain.Constants;
using game_x.share.Extensions;

namespace game_x.persistence.Repo;

public class ConversationRepo(GameXContext context): IConversationRepo, IRepository
{
    private IQueryable<ConversationItemDto> BuildConversationListing(
        IQueryable<Conversation> query, 
        string userId,
        bool? getUnreadCount = null,
        bool? isBackOffice = null)
    {
        var baseQuery =
                query
                .AsNoTracking()
                .Where(c =>
                    c.Type != ConversationType.Public && c.Messages.Any()
                )
                .Select(c => new
                {
                    Conv = c,
                    LastMessageId = c.Messages
                        .Where(m => !m.IsDeleted)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.Id)
                        .FirstOrDefault()
                });
        
        return
            from x in baseQuery
            join m in context.Messages
                    .AsNoTracking()
                    .Include(m => m.SenderUser)
                        .ThenInclude(u => u != null ? u.Avatar : null)
                on x.LastMessageId equals m.Id into lm
            from lastMessage in lm.DefaultIfEmpty()
            select new ConversationItemDto
            {
                Id = x.Conv.PublicId,
                GuestId = x.Conv.GuestId,
                Type = x.Conv.Type,
                Status = x.Conv.Status,
                CustomerId = x.Conv.CustomerId,
                CustomerDisplayName = x.Conv.Customer != null
                    ? x.Conv.Customer.Nickname
                    : null,
                CustomerAvatar = x.Conv.Customer != null
                    ? x.Conv.Customer.Avatar
                    : null,
                LastResolvedAt = x.Conv.LastResolvedAt,
                LastResolvedMessageId = x.Conv.LastResolvedMessageId,
                LastMessageAt = x.Conv.LastMessageAt,
                LastMessage = lastMessage == null
                    ? null
                    : new LastMessageItemDto
                    {
                        Id = lastMessage.Id,
                        PublicId = lastMessage.PublicId,
                        SenderActorId = lastMessage.SenderActorId,
                        SenderRole = lastMessage.SenderRole,
                        SenderName = lastMessage.SenderUser != null
                            ? 
                            lastMessage.SenderUser.Nickname.IsNotNullOrEmpty()
                            ? lastMessage.SenderUser.Nickname
                            : lastMessage.SenderUser.UserName
                            : string.Empty,
                        SenderAvatar = lastMessage.SenderUser != null
                            ? lastMessage.SenderUser.Avatar
                            : null,
                        SentAt = lastMessage.SentAt,
                        Text = lastMessage.Text,
                        Kind = lastMessage.Kind
                    },
                IsHidden = false,
                UnreadCount = getUnreadCount == true 
                    ? context.Messages.Count(m =>
                        m.Conversation.Id == x.Conv.Id 
                        && (isBackOffice == true 
                            ? x.Conv.LastResolvedMessageId == null || m.Id > x.Conv.LastResolvedMessageId 
                            : context.ConversationMembers.Any(cm =>
                                cm.Conversation.Id == x.Conv.Id &&
                                cm.UserId == userId &&
                                (cm.LastReadMessageId == null || m.Id > cm.LastReadMessageId)
                            ))
                        ) 
                    : null
            };
    }
    
    // ---- Cursor-based queue for unassigned support conversations (next-only) ----
    public IQueryable<ConversationItemDto> GetUnassignedQueueByCursorAsync(string userId, CancellationToken ct = default)
    {
        // --- Base feed: only open, unassigned support conversations ---
        var query = context.Conversations
            .Where(c => c.Type == ConversationType.Support
                     && c.Status == ConversationStatus.Open
                     && c.AssignedAgentId == null);
        return BuildConversationListing(query, userId);
    }
    
    public IQueryable<ConversationItemDto> GetSupportConversationsAsync(string userId, CancellationToken ct = default)
    {
        IQueryable<Conversation> query = context.Conversations
            .Where(c => c.Type == ConversationType.Support && c.Status == ConversationStatus.Claimed);
        return BuildConversationListing(query, userId, true, true);
    }

    public IQueryable<ConversationItemDto> GetHiddenConversationsForClientAsync(string userId, CancellationToken ct = default)
    {
        // Retrieve all private hidden conversations and group chats
        Expression<Func<Conversation, bool>> minePredicate =
            c => context.ConversationMembers.Any(m => 
                m.ConversationId == c.Id && 
                m.UserId == userId &&
                m.IsHidden == true);
        IQueryable<Conversation> query = context.Conversations.Where(minePredicate);
        return BuildConversationListing(query, userId);
    }

    public IQueryable<ConversationItemDto> GetMyConversationsForClientAsync(string userId, CancellationToken ct = default)
    {
        // Retrieve all private conversations and group chats
        Expression<Func<Conversation, bool>> minePredicate =
            c => c.CustomerId == userId
              || context.ConversationMembers.Any(m => 
                  m.ConversationId == c.Id && 
                  m.UserId == userId &&
                  m.IsHidden != true);
        IQueryable<Conversation> query = context.Conversations.Where(minePredicate);
        return BuildConversationListing(query, userId, true);
    }

    public async Task<ConversationItemDto?> GetMyConversationsForGuestAsync(string guestId, CancellationToken ct = default)
    {
        return await context.Conversations
            .AsTracking()
            .Where(x => x.GuestId == guestId && x.Type == ConversationType.Support)
            .Select(c => new
            {
                Conv = c,
                LastMessage = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()
            })
            .Select(x => new ConversationItemDto
            {
                Id = x.Conv.PublicId,
                GuestId = x.Conv.GuestId,
                Type = x.Conv.Type,
                Status = x.Conv.Status,
                CustomerId = x.Conv.CustomerId,
                CustomerDisplayName = x.Conv.Customer != null ? x.Conv.Customer.Nickname : null,
                CustomerAvatar = x.Conv.Customer != null ? x.Conv.Customer.Avatar: null,
                LastMessageAt = x.Conv.LastMessageAt,
                LastGuestReadMessageId = x.Conv.LastGuestReadMessageId,
                LastGuestReadAt = x.Conv.LastGuestReadAt,
                LastMessage = x.LastMessage == null
                    ? null
                    : new LastMessageItemDto
                    {
                        Id = x.LastMessage.Id,
                        PublicId = x.LastMessage.PublicId,
                        SenderActorId = x.LastMessage.SenderActorId,
                        SenderRole = x.LastMessage.SenderRole,
                        SenderName = x.LastMessage.SenderUser != null
                            ? x.LastMessage.SenderUser.Nickname
                            : string.Empty,
                        SenderAvatar = x.LastMessage.SenderUser != null
                            ? x.LastMessage.SenderUser.Avatar
                            : null,
                        SentAt = x.LastMessage.SentAt,
                        Text = x.LastMessage.Text,
                        Kind = x.LastMessage.Kind
                    },
                UnreadCount = context.Messages.Count(m =>
                    m.Conversation.Id == x.Conv.Id
                    && x.Conv.LastGuestReadMessageId == null || m.Id > x.Conv.LastGuestReadMessageId 
                )
            })
            .FirstOrDefaultAsync(ct);
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
    
    public async Task<IReadOnlyCollection<ConversationUnreadDto>> GetSupportConvUnreadAsync(CancellationToken ct = default)
    {
        var baseQuery = context.Conversations
            .AsTracking()
            .Where(x => x.Type == ConversationType.Support);
            
        int claimedConvUnread = await baseQuery
            .Where(c => c.Status == ConversationStatus.Claimed)
            .SumAsync(x => x.Messages.Count(m => x.LastResolvedMessageId == null || m.Id > x.LastResolvedMessageId), ct);

        int openedConvUnread = await baseQuery.CountAsync(x => x.Status == ConversationStatus.Open, ct);
        
        return
        [
            new ConversationUnreadDto(ConversationStatus.Claimed, claimedConvUnread),
            new ConversationUnreadDto(ConversationStatus.Open, openedConvUnread)
        ];
    }
    
    public async Task<int> CountSupportConvUnreadAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Conversations
            .AsTracking()
            .Where(x => x.Type == ConversationType.Support && x.PublicId == id)
            .SumAsync(x => x.Messages.Count(m => 
                x.LastResolvedMessageId == null || m.Id > x.LastResolvedMessageId), ct);
    }
    
    public async Task<int> CountConvUnreadByUserIdAsync(string userId, Guid id, CancellationToken ct = default)
    {
        return await context.Conversations
            .AsTracking()
            .Where(x => x.CustomerId == userId && x.PublicId == id)
            .SelectMany(c =>
                context.ConversationMembers
                    .Where(m => m.ConversationId == c.Id && m.UserId == userId)
                    .SelectMany(m =>
                        c.Messages.Where(msg =>
                            m.LastReadMessageId == null || msg.Id > m.LastReadMessageId)))
            .CountAsync(ct);
    }
    
    public async Task<int> CountConvUnreadByGuestIdAsync(string guestId, Guid id, CancellationToken ct = default)
    {
        return await context.Conversations
            .AsTracking()
            .Where(x => x.GuestId == guestId && x.PublicId == id)
            .SelectMany(c => c.Messages
                .Where(m => c.LastGuestReadMessageId == null || m.Id > c.LastGuestReadMessageId))
            .CountAsync(ct);
    }
    
    public async Task<ConversationItemDto> GetConvByIdAsync(Guid convId, CancellationToken ct = default)
    {
        var conv = await context.Conversations
            .AsTracking()
            .Where(x => x.PublicId == convId)
            .Select(c => new
            {
                Conv = c,
                LastMessage = c.Messages.OrderByDescending(m => m.SentAt).FirstOrDefault()
            })
            .Select(x => new ConversationItemDto
            {
                Id = x.Conv.PublicId,
                GuestId = x.Conv.GuestId,
                Type = x.Conv.Type,
                Status = x.Conv.Status,
                CustomerId = x.Conv.CustomerId,
                CustomerDisplayName = x.Conv.Customer != null ? x.Conv.Customer.Nickname : null,
                CustomerAvatar =  x.Conv.Customer != null ? x.Conv.Customer.Avatar: null,
                LastResolvedAt = x.Conv.LastResolvedAt,
                LastResolvedMessageId = x.Conv.LastResolvedMessageId,
                LastGuestReadMessageId = x.Conv.LastGuestReadMessageId,
                LastGuestReadAt = x.Conv.LastGuestReadAt,
                LastMessageAt = x.Conv.LastMessageAt,
                LastMessage = x.LastMessage == null
                ? null
                : new LastMessageItemDto
                {
                    Id = x.LastMessage.Id,
                    PublicId = x.LastMessage.PublicId,
                    SenderActorId = x.LastMessage.SenderActorId,
                    SenderRole = x.LastMessage.SenderRole,
                    SenderName = x.LastMessage.SenderUser != null
                        ? x.LastMessage.SenderUser.Nickname
                        : string.Empty,
                    SenderAvatar = x.LastMessage.SenderUser != null
                        ? x.LastMessage.SenderUser.Avatar
                        : null,
                    SentAt = x.LastMessage.SentAt,
                    Text = x.LastMessage.Text,
                    Kind = x.LastMessage.Kind
                },
            })
            .FirstOrDefaultAsync(ct);

        if(conv == null)
            throw new NotFoundException(MessageCode.Chatting.ConversationNotFound);
        return conv;
    }
    
    public async Task<Conversation> GetPublicConvAsync(CancellationToken ct = default)
    {
        return await context.Conversations
                       .AsTracking()
                       .FirstOrDefaultAsync(c => c.Type == ConversationType.Public, ct)
                   ?? throw new NotFoundException(MessageCode.Chatting.ConversationNotFound);
    }
    
    public async Task<ConversationItemDto> GetConvByIdAndUserIdAsync(Guid convId, string userId, CancellationToken ct = default)
    {
        var conv = await context.Conversations
            .AsTracking()
            .Where(c => c.PublicId == convId && c.Members.Any(cm => cm.UserId == userId))
            .Select(c => new
            {
                Conv = c,
                Member = context.ConversationMembers
                    .Where(x => x.ConversationId == c.Id)
                    .FirstOrDefault(m => m.UserId == userId),
                LastMessage = context.Messages
                    .Where(x => x.ConversationId == c.Id)
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefault()
            })
            .Select(x => new ConversationItemDto
            {
                Id = x.Conv.PublicId,
                GuestId = x.Conv.GuestId,
                Type = x.Conv.Type,
                Status = x.Conv.Status,
                CustomerId = x.Conv.CustomerId,
                CustomerDisplayName = x.Conv.Customer != null ? x.Conv.Customer.Nickname : null,
                CustomerAvatar = x.Conv.Customer != null ? x.Conv.Customer.Avatar : null,
                LastUserReadAt = x.Member!.LastSeenAt,
                LastUserReadMessageId = x.Member.LastReadMessageId,
                LastResolvedAt = x.Conv.LastResolvedAt,
                LastResolvedMessageId = x.Conv.LastResolvedMessageId,
                LastGuestReadMessageId = x.Conv.LastGuestReadMessageId,
                LastGuestReadAt = x.Conv.LastGuestReadAt,
                LastMessageAt = x.Conv.LastMessageAt,
                LastMessage = x.LastMessage == null
                ? null
                : new LastMessageItemDto
                {
                    Id = x.LastMessage.Id,
                    PublicId = x.LastMessage.PublicId,
                    SenderActorId = x.LastMessage.SenderActorId,
                    SenderRole = x.LastMessage.SenderRole,
                    SenderName =
                        x.LastMessage.SenderUser != null
                            ? x.LastMessage.SenderUser.Nickname
                            : string.Empty,
                    SenderAvatar = x.LastMessage.SenderUser != null
                        ? x.LastMessage.SenderUser.Avatar
                        : null,
                    SentAt = x.LastMessage.SentAt,
                    Text = x.LastMessage.Text,
                    Kind = x.LastMessage.Kind
                },
                IsHidden = x.Member != null ? x.Member.IsHidden : false
            })
            .FirstOrDefaultAsync(ct);
        
        if(conv == null)
            throw new NotFoundException(MessageCode.Chatting.ConversationNotFound);
        return conv;
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
    
    public async Task UpdateAsync(Guid publicId, Action<Conversation> updateAction, CancellationToken ct = default)
    {
        var conv = await context.Conversations
            .FirstOrDefaultAsync(c => c.PublicId == publicId, ct)
                ?? throw new NotFoundException(MessageCode.Chatting.ConversationNotFound);

        updateAction.Invoke(conv);
        await context.SaveChangesAsync(ct);
    }
}