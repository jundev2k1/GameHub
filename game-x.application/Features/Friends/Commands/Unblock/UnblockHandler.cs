using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Friendship.OnFriendUnblocked;
using game_x.application.Features.Friends.Dtos;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Friends.Commands.Unblock;

public sealed class UnblockHandler(
    IUnitOfWork unitOfWork,
    IUserRepo useRepo,
    ISocialLinkRepo socialLinkRepo,
    IUserAccessor userAccessor,
    IApplicationEventDispatcher dispatcher,
    IFileManagerCacheService fileCache,
    ILogger<SocialLink> logger) : IRequestHandler<UnblockCommand, Unit>
{
    public async Task<Unit> Handle(UnblockCommand req, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();
        if (me == req.TargetUserId) throw new BadRequestException(MessageCode.Chatting.FailToTargetMyself);

        var isExistedTargetUser = await useRepo.IsExistUserIdAsync(req.TargetUserId, ct);
        if(!isExistedTargetUser)
            throw new NotFoundException(MessageCode.User.UserNotFound);
        
        var (min, max) = SocialLinkPair.Normalize(me, req.TargetUserId);
        var existed = await socialLinkRepo.GetByKeyPairAsync(min, max, ct);
        
        if(existed == null || existed.Kind != SocialLinkKind.Block || existed.BlockerUserId != me)
            throw new NotFoundException(MessageCode.Chatting.SocialLinkNotFound);

        try
        {
            await socialLinkRepo.UpdateAsync(existed.PublicId, x =>
            {
                x.OnUnBlock();
            }, ct);
            
            await unitOfWork.CommitAsync(ct);
            var existedAvatarUrl = 
                existed.BlockerUser?.Avatar != null 
                    ? await fileCache.GetFileUrl(existed.BlockerUser.Avatar, ct) 
                    : null;
            
            await dispatcher.Publish(new OnFriendUnblockedEvent(existed.Adapt<SocialLinkDto>() with {BlockerAvatarUrl = existedAvatarUrl}), ct);
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