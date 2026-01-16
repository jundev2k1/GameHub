using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUnfriend;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Friends.Commands.Unfriend;

public sealed class UnfriendHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    ISocialLinkRepo socialLinkRepo,
    IConversationRepo conversationRepo,
    IUserAccessor userAccessor,
    IFileManagerCacheService fileCache,
    IApplicationEventDispatcher dispatcher,
    ILogger<SocialLink> logger) : IRequestHandler<UnfriendCommand, Unit>
{
    public async Task<Unit> Handle(UnfriendCommand req, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();
        if (me == req.TargetUserId) throw new BadRequestException(MessageCode.Chatting.FailToTargetMyself);

        var isExistedTargetUser = await userRepo.IsExistUserIdAsync(req.TargetUserId, ct);
        if(!isExistedTargetUser)
            throw new NotFoundException(MessageCode.User.UserNotFound);
        
        var (min, max) = SocialLinkPair.Normalize(me, req.TargetUserId);
        var existed = await socialLinkRepo.GetByKeyPairAsync(min, max, ct);
        
        if (existed is null || !existed.IsFriend) 
            throw new BadRequestException(MessageCode.Chatting.StillNotFriend);
        
        var existedConv = await conversationRepo.FindForPairAsync(me, req.TargetUserId, ct);
        
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            await socialLinkRepo.UpdateAsync(existed.PublicId, x => { x.State = SocialLinkState.Declined; }, ct);
            if (existedConv != null)
            {
                await conversationRepo.UpdateAsync(existedConv.PublicId, x =>
                {
                    x.Status = ConversationStatus.Closed;
                }, ct);
            }
            
            User? actor = existed.RequesterUserId == me ? existed.RequesterUser : existed.AddresseeUser;
            if (actor is null) throw new NotFoundException(MessageCode.User.UserNotFound);
            var avatarUrl = actor.Avatar != null ? await fileCache.GetFileUrl(actor.Avatar, ct) : null;
        
            await unitOfWork.CommitAsync(ct);
            await dispatcher.Publish(new OnUnfriendEvent(new UnfriendSignalDto
            (
                LinkId: existed.PublicId,
                UnfrienderId: actor.Id,
                UnfrienderNickname: actor.Nickname,
                UnfrienderAvatarUrl: avatarUrl,
                UnfriendedUserId: existed.RequesterUserId == actor.Id ? existed.AddresseeUserId : existed.RequesterUserId
            )), ct);
            return Unit.Value;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            await unitOfWork.RollbackAsync(ct);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
}