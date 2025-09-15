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
    ILogger<SocialLink> logger) : IRequestHandler<SendFriendRequestCommand, Unit>
{
    public async Task<Unit> Handle(SendFriendRequestCommand req, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();
        if (me == req.TargetUserId) throw new BadRequestException(MessageCode.Chatting.FailToFriendYourself);

        var isExistedTargetUser = await useRepo.IsExistUserIdAsync(req.TargetUserId, ct);
        if(!isExistedTargetUser)
            throw new NotFoundException(MessageCode.User.UserNotFound);
        
        var (min, max) = SocialLinkPair.Normalize(me, req.TargetUserId);
        var existed = await socialLinkRepo.GetByKeyPairAsync(min, max, SocialLinkKind.Friendship, ct);
        
        if (existed != null)
        {
            if (existed.State == SocialLinkState.Pending)
                throw new BadRequestException(MessageCode.Chatting.WaitToAccept);
            if (existed.State == SocialLinkState.Accepted) 
                throw new BadRequestException(MessageCode.Chatting.AlreadyFriend);
            
            // Reopen if previously rejected
            existed.State = SocialLinkState.Pending;
            existed.RequesterUserId = me;
            existed.AddresseeUserId = req.TargetUserId;
            existed.RespondedAt = null;
            
            try
            {
                await unitOfWork.SaveChangesAsync(ct);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                await unitOfWork.RollbackAsync(ct);
                throw new BadRequestException(MessageCode.System.SystemError);
            }
            await dispatcher.Publish(new OnSendFriendRequestEvent(existed.Adapt<SocialLinkDto>()), ct);
            return Unit.Value;
        }
        
        var link = SocialLink.Create(
            min: min,
            max: max,
            kind: SocialLinkKind.Friendship,
            state: SocialLinkState.Pending,
            requesterUserId: me,
            addresseeUserId: req.TargetUserId);

        try
        {
            await socialLinkRepo.AddAsync(link, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            await unitOfWork.RollbackAsync(ct);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
        
        var createdLink = await socialLinkRepo.GetByKeyPairAsync(min, max, SocialLinkKind.Friendship, ct);

        await dispatcher.Publish(new OnSendFriendRequestEvent(createdLink.Adapt<SocialLinkDto>()), ct);
        return Unit.Value;
    }
}