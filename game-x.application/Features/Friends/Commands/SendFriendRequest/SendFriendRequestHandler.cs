using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnSendFriendRequest;
using game_x.application.Features.Friends.Dtos;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Friends.Commands.SendFriendRequest;

public sealed class SendFriendRequestHandler(
    IUnitOfWork unitOfWork,
    IUserRepo useRepo,
    ISocialLinkRepo socialLinkRepo,
    IUserAccessor userAccessor,
    IApplicationEventDispatcher dispatcher,
    IFileManagerCacheService fileCache,
    ILogger<SocialLink> logger) : IRequestHandler<SendFriendRequestCommand, Unit>
{
    public async Task<Unit> Handle(SendFriendRequestCommand req, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();
        if (me == req.TargetUserId) throw new BadRequestException(MessageCode.Chatting.FailToTargetMyself);

        var isExistedTargetUser = await useRepo.IsExistUserIdAsync(req.TargetUserId, ct);
        if(!isExistedTargetUser)
            throw new NotFoundException(MessageCode.User.UserNotFound);
        
        var (min, max) = SocialLinkPair.Normalize(me, req.TargetUserId);
        var existed = await socialLinkRepo.GetByKeyPairAsync(min, max, ct);
        
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            SocialLinkDto? socialLinkDto;
            if (existed != null)
            {
                if (existed.Kind == SocialLinkKind.Block)
                    throw new BadRequestException(MessageCode.Chatting.SocialLinkBlocked);   
                if (existed.State == SocialLinkState.Pending)
                    throw new BadRequestException(MessageCode.Chatting.WaitToAccept);
                if (existed.State == SocialLinkState.Accepted) 
                    throw new BadRequestException(MessageCode.Chatting.AlreadyFriend);
                
                await socialLinkRepo.UpdateAsync(existed.PublicId, x =>
                {
                    x.Reset();
                    x.State = SocialLinkState.Pending;
                    x.RequesterUserId = me;
                    x.AddresseeUserId = req.TargetUserId;
                    x.RespondedAt = null;
                }, ct);
                await unitOfWork.CommitAsync(ct);
                
                var existedAvatarUrl = 
                    existed.RequesterUser?.Avatar != null 
                        ? await fileCache.GetFileUrl(existed.RequesterUser.Avatar, ct) 
                        : null;

                socialLinkDto = existed.Adapt<SocialLinkDto>() with { RequesterAvatarUrl = existedAvatarUrl };
            }
            else
            {
                var link = SocialLink.Create(
                    min: min,
                    max: max,
                    kind: SocialLinkKind.Friendship,
                    state: SocialLinkState.Pending,
                    requesterUserId: me,
                    addresseeUserId: req.TargetUserId);
                
                await socialLinkRepo.AddAsync(link, ct);
                await unitOfWork.CommitAsync(ct);
                
                var createdLink = await socialLinkRepo.GetByKeyPairAsync(min, max, ct);
                var requesterAvatarUrl = 
                    createdLink?.RequesterUser?.Avatar != null 
                        ? await fileCache.GetFileUrl(createdLink.RequesterUser.Avatar, ct) 
                        : null;

                socialLinkDto = createdLink.Adapt<SocialLinkDto>() with { RequesterAvatarUrl = requesterAvatarUrl };
            }
            await dispatcher.Publish(new OnSendFriendRequestEvent(socialLinkDto), ct);
            return Unit.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}