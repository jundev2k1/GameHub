using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnRespondRequest;
using game_x.application.Features.Friends.Dtos;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Friends.Commands.RespondFriendRequest;

public class RespondFriendRequestHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    ISocialLinkRepo socialLinkRepo,
    IApplicationEventDispatcher dispatcher,
    ILogger<SocialLink> logger): IRequestHandler<RespondFriendRequestCommand, Unit>
{
    public async Task<Unit> Handle(RespondFriendRequestCommand cmd, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();
        var link = await socialLinkRepo.GetByIdAsync(cmd.LinkPublicId, SocialLinkKind.Friendship, ct);

        if (link.State != SocialLinkState.Pending)
            throw new BadRequestException(MessageCode.Chatting.FriendRequestAlreadyRespond);
        if (link.AddresseeUserId != me)
            throw new BadRequestException(MessageCode.Chatting.NotAddressee);

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var updatedLink = await socialLinkRepo.UpdateAsync(link.PublicId, x => { x.Respond(cmd.Accept); }, ct);
            await unitOfWork.CommitAsync(ct);
            await dispatcher.Publish(new OnRespondRequestEvent(updatedLink.Adapt<SocialLinkDto>()), ct);
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