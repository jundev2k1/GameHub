using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnFriendBlocked;
using game_x.application.Features.Friends.Dtos;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Friends.Commands.Block;

public sealed class BlockHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    ISocialLinkRepo socialLinkRepo,
    IConversationRepo conversationRepo,
    IUserAccessor userAccessor,
    IApplicationEventDispatcher dispatcher,
    IFileManagerCacheService fileCache,
    ILogger<SocialLink> logger) : IRequestHandler<BlockCommand, Unit>
{
    public async Task<Unit> Handle(BlockCommand req, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();
        if (me == req.TargetUserId) throw new BadRequestException(MessageCode.Chatting.FailToTargetMyself);

        var isExistedTargetUser = await userRepo.IsExistUserIdAsync(req.TargetUserId, ct);
        if(!isExistedTargetUser)
            throw new NotFoundException(MessageCode.User.UserNotFound);
        
        var (min, max) = SocialLinkPair.Normalize(me, req.TargetUserId);
        var existed = await socialLinkRepo.GetByKeyPairAsync(min, max, ct);
        
        if (existed?.Kind == SocialLinkKind.Block)
            throw new BadRequestException(MessageCode.Chatting.SocialLinkBlocked);
        
        var existedConv = await conversationRepo.FindForPairAsync(me, req.TargetUserId, ct);
        
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            SocialLinkDto? linkDto;
            if (existedConv != null)
            {
                await conversationRepo.PatchUpdateAsync(existedConv.PublicId, x =>
                {
                    x.Status = ConversationStatus.Closed;
                }, ct);
            }
            
            if (existed != null)
            {
                await socialLinkRepo.UpdateAsync(existed.PublicId, x =>
                {
                    x.Kind = SocialLinkKind.Block;
                    x.State = SocialLinkState.Blocked;
                    x.BlockerUserId = me;
                    x.BlockedUserId = req.TargetUserId;
                    x.RespondedAt = DateTime.UtcNow;
                }, ct);
                await unitOfWork.CommitAsync(ct);
                var existedAvatar = 
                    existed.BlockerUser?.Avatar != null 
                        ? await fileCache.GetImageUrl(existed.BlockerUser.Avatar, ct) 
                        : null;

                linkDto = existed.Adapt<SocialLinkDto>() with { BlockerAvatarUrl = existedAvatar?.Url };
            }
            else
            {
                var link = SocialLink.Create(
                    min: min,
                    max: max,
                    kind: SocialLinkKind.Block,
                    state: SocialLinkState.Blocked,
                    blockerUserId: me,
                    blockedUserId: req.TargetUserId,
                    respondedAt: DateTime.UtcNow);
                await socialLinkRepo.AddAsync(link, ct);
                await unitOfWork.CommitAsync(ct);
                var createdLink = await socialLinkRepo.GetByKeyPairAsync(min, max, ct);
                var blockerAvatar = 
                    createdLink?.BlockerUser?.Avatar != null 
                        ? await fileCache.GetImageUrl(createdLink.BlockerUser.Avatar, ct) 
                        : null;
        
                linkDto = createdLink.Adapt<SocialLinkDto>() with {BlockerAvatarUrl = blockerAvatar?.Url};
            }
            
            await dispatcher.Publish(new OnFriendBlockedEvent(linkDto), ct);
            return Unit.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            await unitOfWork.RollbackAsync(ct);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
}