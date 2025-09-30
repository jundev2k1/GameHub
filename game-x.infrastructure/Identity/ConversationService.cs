using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Chat.Dtos;
using game_x.infrastructure.Extensions;
using game_x.share.Helper;

namespace game_x.infrastructure.Identity;

public sealed class ConversationService(
    IUserAccessor userAccessor,
    IUnitOfWork unitOfWork,
    IConversationRepo conversationRepo,
    IConversationMemberRepo conversationMemberRepo,
    ISocialLinkRepo socialLinkRepo,
    IUserRepo userRepo,
    IAppLogger<Conversation> logger,
    IFileManagerCacheService fileCache): IConversationService, IServices
{
    public async Task<Guid> EnsureForPublic(CancellationToken ct)
    {
        var me = userAccessor.GetUserId();
        var existedConv = await conversationRepo.FindPublicAsync(ct);
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            Guid returnConvId;
            if (existedConv != null)
            {
                bool isExistedMember = await conversationMemberRepo.CheckExistMemberAsync(existedConv.Id, me, ct);
                if (!isExistedMember)
                    existedConv.Members.Add(ConversationMember.Create(existedConv, me, RoleInConversation.Member));
                returnConvId = existedConv.PublicId;
            }
            else
            {
                var conv = Conversation.Create(ConversationType.Public);
                await conversationRepo.AddAsync(conv, ct);
                conv.Members.Add(ConversationMember.Create(
                    conv: conv, 
                    userId: me, 
                    role: RoleInConversation.Member,
                    lastDeliveredAt: DateTime.UtcNow));
                returnConvId = conv.PublicId;
            }
            await unitOfWork.CommitAsync(ct);
            return returnConvId;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
    
    public async Task<Guid> EnsureForPair(string me, string targetedUserId, CancellationToken ct)
    {
        if (me == targetedUserId)
            throw new BadRequestException(MessageCode.Chatting.FailToTargetMyself);

        var isExistedTargetUser = await userRepo.IsExistUserIdAsync(targetedUserId, ct);
        if(!isExistedTargetUser)
            throw new NotFoundException(MessageCode.User.UserNotFound);
        
        var (min, max) = SocialLinkPair.Normalize(me, targetedUserId);
        var link = await socialLinkRepo.GetByKeyPairAsync(min, max, ct);
        if(link == null || !link.IsFriend)
            throw new NotFoundException(MessageCode.Chatting.StillNotFriend);
        
        // Find a direct conversation with exactly two members, A and B
        var existedConv = await conversationRepo.FindForPairAsync(me, targetedUserId, ct);
        if (existedConv != null) return existedConv.PublicId;

        // Create new if missing
        var conv = Conversation.Create(ConversationType.Direct);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await conversationRepo.AddAsync(conv, ct);
            conv.Members.Add(ConversationMember.Create(conv, me, RoleInConversation.Member));
            conv.Members.Add(ConversationMember.Create(conv, targetedUserId, RoleInConversation.Member));
        }, ct);
        
        return conv.PublicId;
    }
    
    public async Task<Guid> EnsureForSupport(string actorId, string? userId, CancellationToken ct)
    {
        // Each customer has only one conversation with customer support; if none exists, a new one will be created
        var conv = await conversationRepo.GetSupportConversationAsync(actorId, ct);
        // Only user need join into a group (guest is not allowed)
        ConversationMember? convMember = null;
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            if (conv is null)
            {
                conv = Conversation.Create(
                    type: ConversationType.Support,
                    senderUserId: userId,
                    senderGuestId: userId is null ? actorId : null);
                
                await conversationRepo.AddAsync(conv, ct);
                if (userId is not null)
                {
                    convMember = ConversationMember.Create(
                        conv: conv,
                        userId: userId,
                        role: RoleInConversation.Member);
        
                    await conversationMemberRepo.AddAsync(convMember, ct);
                }
            }

            if (convMember is null && userId is not null)
            {
                var exists = await conversationMemberRepo.CheckExistMemberAsync(conv.Id, userId, ct);
                if (!exists)
                {
                    convMember = ConversationMember.Create(
                        conv: conv,
                        userId: userId,
                        role: RoleInConversation.Member);
        
                    await conversationMemberRepo.AddAsync(convMember, ct);
                }
            }
            await unitOfWork.CommitAsync(ct);
        }
        catch(Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError("Failed to create the message: {Ex}", ex);
            throw new BadRequestException(MessageCode.System.SystemError);
        }

        return conv.PublicId;
    }
    
    public async Task<CursorResult<SupportConversationDto>> GetUnassignedQueueByCursorAsync(
        int limit,
        string? cursor,
        CancellationToken ct = default)
    {
        var me = userAccessor.GetUserId();
        var src = conversationRepo.GetUnassignedQueueByCursorAsync(me, ct);
        var result = await BuildConversationListing(src, limit, cursor, ct);
        var dtoItems = result.Items.Adapt<IEnumerable<SupportConversationDto>>();
        return result.Transform(dtoItems);
    }
    
    public async Task<CursorResult<SupportConversationDto>> GetSupportConversationsAsync(
        int limit,
        string? cursor,
        CancellationToken ct = default)
    {
        var me = userAccessor.GetUserId();
        var src = conversationRepo.GetSupportConversationsAsync(me, ct);
        var result = await BuildConversationListing(src, limit, cursor, ct);
        var dtoItems = result.Items.Adapt<IEnumerable<SupportConversationDto>>();
        return result.Transform(dtoItems);
    }
    
    public async Task<CursorResult<ListedConversationDto>> GetHiddenConversationsForClientAsync(
        string userId,
        int limit,
        string? cursor,
        CancellationToken ct = default)
    {
        var src = conversationRepo.GetHiddenConversationsForClientAsync(userId, ct);
        var result = await BuildConversationListing(src, limit, cursor, ct);
        var dtoItems = result.Items.Adapt<IEnumerable<ListedConversationDto>>();
        return result.Transform(dtoItems);
    }
    
    public async Task<CursorResult<ListedConversationDto>> GetMyConversationsForClientAsync(
        string userId,
        int limit,
        string? cursor,
        CancellationToken ct = default)
    {
        var src = conversationRepo.GetMyConversationsForClientAsync(userId, ct);
        var result = await BuildConversationListing(src, limit, cursor, ct);
        var dtoItems = result.Items.Adapt<IEnumerable<ListedConversationDto>>();
        return result.Transform(dtoItems);
    }

    public async Task<ListedConversationDto?> GetMyConversationsForGuestAsync(string guestId, CancellationToken ct = default)
    {
        var src = await conversationRepo.GetMyConversationsForGuestAsync(guestId, ct);
        return src?.Adapt<ConversationDto>().Adapt<ListedConversationDto>();
    }
    
    public async Task<ConversationDetailDto> GetConversationDetailAsync(Guid convId, CancellationToken ct = default)
    {
        var conv = await conversationRepo.GetConversationDetailAsync(convId, ct);
        var convDto = await BuildConversationDto(conv, ct);
        return convDto.Adapt<ConversationDetailDto>();
    }
    
    // --- Helpers ---
    private async Task<CursorResult<ConversationDto>> BuildConversationListing(
        IQueryable<Conversation> src,
        int limit,
        string? cursor,
        CancellationToken ct)
    {
        var fp = CursorHelper.ComputeFp($"q:");

        var result = await SeekCursorBuilder<Conversation>
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
            .ExecuteAsync(c => c, ct);

        var dtoItems = await Task.WhenAll(
            result.Items.Select(async conv => await BuildConversationDto(conv, ct)));
        return result.Transform(dtoItems);
    }

    private async Task<string?> GetAvatarUrl(MediaFile? file, CancellationToken ct)
    {
        string? avatarUrl = null;
        if (file != null)
        {
            avatarUrl = await fileCache.GetFileUrl(file, ct);
        }
        return avatarUrl;
    }
    
    private async Task<ConversationDto> BuildConversationDto(Conversation conv, CancellationToken ct)
    {
        var dto = conv.Adapt<ConversationDto>();
        return dto with
        {
            CustomerAvatarUrl = await GetAvatarUrl(conv.Customer?.Avatar, ct),
            LastUserAvatarUrl = await GetAvatarUrl(conv.Messages.FirstOrDefault()?.SenderUser?.Avatar, ct)
        };
    }
}