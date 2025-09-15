using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Friends.Commands.RespondFriendRequest;

public class RespondFriendRequestHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    ISocialLinkRepo socialLinkRepo,
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
            await socialLinkRepo.PatchUpdateAsync(link.PublicId, x => { x.Respond(cmd.Accept); }, ct);
            await unitOfWork.CommitAsync(ct);
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