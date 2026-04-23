using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Friendship.OnRespondRequest;
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
        
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var link = await Validate(cmd.LinkPublicId, ct);
        
            var existedConv = link.RequesterUserId != null ? await conversationRepo.FindForPairAsync(me, link.RequesterUserId, ct) : null;
            
            await socialLinkRepo.UpdateAsync(link.PublicId, x => { x.Respond(cmd.Accept); }, ct);

            if (existedConv == null)
                await CreateConversation(me, link.RequesterUserId!, ct);
            else if (existedConv.Status == ConversationStatus.Closed)
                await conversationRepo.UpdateAsync(existedConv.PublicId, x => { x.OnOpen(); }, ct);
            
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
            throw;
        }
    }

    private async Task<SocialLink> Validate(Guid linkId, CancellationToken ct = default)
    {
        var me = userAccessor.GetUserId();
        var link = await socialLinkRepo.GetForUpdateAsync(linkId, ct);

        if(link.Kind == SocialLinkKind.Block)
            throw new NotFoundException(MessageCode.Chatting.SocialLinkBlocked);
        if(link.Kind != SocialLinkKind.Friendship)
            throw new NotFoundException(MessageCode.Chatting.SocialLinkNotFound);
        if (link.State != SocialLinkState.Pending)
            throw new BadRequestException(MessageCode.Chatting.FriendRequestAlreadyRespond);
        if (link.AddresseeUserId != me)
            throw new BadRequestException(MessageCode.Chatting.NotAddressee);
        
        return link;
    }

    private async Task CreateConversation(string me, string targetedUserId, CancellationToken ct = default)
    {
        var conv = Conversation.Create(ConversationType.Direct);
        await conversationRepo.AddAsync(conv, ct);
        conv.Members.Add(ConversationMember.Create(conv, me, RoleInConversation.Member));
        conv.Members.Add(ConversationMember.Create(conv, targetedUserId, RoleInConversation.Member));
    }
}