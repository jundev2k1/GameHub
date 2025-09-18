using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Chat.Dtos;
using game_x.infrastructure.Extensions;
using game_x.share.Helper;

namespace game_x.infrastructure.Identity;

public sealed class ConversationService(
    IUnitOfWork unitOfWork,
    IConversationRepo conversationRepo, 
    IFileStorageService fileStorage): IConversationService, IServices
{
    public async Task<Guid> EnsureForPair(string userA, string userB, CancellationToken ct)
    {
        if (userA == userB)
            throw new BadRequestException(MessageCode.Chatting.FailToTargetMyself);

        // Find a direct conversation with exactly two members, A and B
        var existing = await conversationRepo.FindForPairAsync(userA, userB, ct);
        if (existing != null) return existing.PublicId;

        // Create new if missing
        var conv = Conversation.Create(ConversationType.Direct);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await conversationRepo.AddAsync(conv, ct);
            conv.Members.Add(ConversationMember.Create(conv, userA, RoleInConversation.Member));
            conv.Members.Add(ConversationMember.Create(conv, userB, RoleInConversation.Member));
        }, ct);
        
        return conv.PublicId;
    }
    
    public async Task<CursorResult<SupportConversationDto>> GetUnassignedQueueByCursorAsync(
        int limit,
        string? cursor,
        CancellationToken ct = default)
    {
        var src = conversationRepo.GetUnassignedQueueByCursorAsync(ct);
        var result = await BuildConversationListing(src, limit, cursor, ct);
        var dtoItems = result.Items.Adapt<IEnumerable<SupportConversationDto>>();
        return result.Transform(dtoItems);
    }
    
    public async Task<CursorResult<SupportConversationDto>> GetSupportConversationsAsync(
        int limit,
        string? cursor,
        CancellationToken ct = default)
    {
        var src = conversationRepo.GetSupportConversationsAsync(ct);
        var result = await BuildConversationListing(src, limit, cursor, ct);
        var dtoItems = result.Items.Adapt<IEnumerable<SupportConversationDto>>();
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
        if (src == null)
            return null;
        
        var dto = src.Adapt<ConversationDto>().Adapt<ListedConversationDto>();
        return dto with { LastMessagePreview = Preview(dto.LastMessagePreview) };
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
    
    /// <summary>Short preview helper</summary>
    private string Preview(string? text)
        => string.IsNullOrWhiteSpace(text) ? "[Attachment]"
            : text.Length <= 140 ? text
            : text[..140];

    private async Task<string?> GetAvatarUrl(MediaFile? file, CancellationToken ct)
    {
        string? avatarUrl = null;
        if (file != null)
        {
            avatarUrl = await fileStorage.GenerateDownloadUrlAsync(
                bucketName: file.BucketName,
                objectName: file.ObjectName,
                ct: ct);
        }
        return avatarUrl;
    }
    
    private async Task<ConversationDto> BuildConversationDto(Conversation conv, CancellationToken ct)
    {
        var dto = conv.Adapt<ConversationDto>();
        return dto with
        {
            LastMessagePreview = Preview(dto.LastMessagePreview), 
            CustomerAvatarUrl = await GetAvatarUrl(conv.Customer?.Avatar, ct),
            LastUserAvatarUrl = await GetAvatarUrl(conv.Messages.FirstOrDefault()?.SenderUser?.Avatar, ct)
        };
    }
}