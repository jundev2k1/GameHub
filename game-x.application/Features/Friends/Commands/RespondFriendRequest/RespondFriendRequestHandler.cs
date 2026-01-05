using game_x.application.Contract.Infrastructure.Caching;
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
    IConversationRepo conversationRepo,
    IApplicationEventDispatcher dispatcher,
    IFileManagerCacheService fileCache,
    ILogger<SocialLink> logger): IRequestHandler<RespondFriendRequestCommand, Unit>
{
    public async Task<Unit> Handle(RespondFriendRequestCommand cmd, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();
        var link = await socialLinkRepo.GetByIdAsync(cmd.LinkPublicId, ct);

        if(link.Kind == SocialLinkKind.Block)
            throw new NotFoundException(MessageCode.Chatting.SocialLinkBlocked);
        if(link.Kind != SocialLinkKind.Friendship)
            throw new NotFoundException(MessageCode.Chatting.SocialLinkNotFound);
        if (link.State != SocialLinkState.Pending)
            throw new BadRequestException(MessageCode.Chatting.FriendRequestAlreadyRespond);
        if (link.AddresseeUserId != me)
            throw new BadRequestException(MessageCode.Chatting.NotAddressee);

        var existedConv = link.RequesterUserId != null ? await conversationRepo.FindForPairAsync(me, link.RequesterUserId, ct) : null;
        
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            await socialLinkRepo.UpdateAsync(link.PublicId, x => { x.Respond(cmd.Accept); }, ct);
            if (existedConv?.Status == ConversationStatus.Closed)
            {
                await conversationRepo.UpdateAsync(existedConv.PublicId, x =>
                {
                    x.Status = ConversationStatus.Open;
                }, ct);
            }
            
            await unitOfWork.CommitAsync(ct);
            var updatedLink = await socialLinkRepo.GetByIdAsync(link.PublicId, ct);
            var avatarUrl = 
                updatedLink.AddresseeUser?.Avatar != null 
                ? await fileCache.GetFileUrl(updatedLink.AddresseeUser.Avatar, ct) 
                : null;
            await dispatcher.Publish(new OnRespondRequestEvent(updatedLink.Adapt<SocialLinkDto>() with {AddresseeAvatarUrl = avatarUrl}), ct);
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