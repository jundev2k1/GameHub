using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Chat.OnSupportConversationClaimed;
using game_x.application.Events.Chat.OnSupportConversationUnread;

namespace game_x.application.Features.Chat.Commands.ClaimConversationById;

public sealed class ClaimConversationByIdHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor, 
    IConversationRepo convRepo,
    IConversationService convService,
    IConversationMemberRepo convMemberRepo,
    IMessageRepo messageRepo,
    IApplicationEventDispatcher eventDispatcher): IRequestHandler<ClaimConversationByIdCommand, Unit>
{
    public async Task<Unit> Handle(ClaimConversationByIdCommand request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var convDto = await convService.GetConvByIdAsync(request.ConversationId, ct);
        if(convDto is null)
            throw new BadRequestException(MessageCode.Chatting.ConversationNotFound);

        if(convDto.Status != ConversationStatus.Open)
            throw new BadRequestException(MessageCode.Chatting.ConversationAlreadyClaimed);

        var lastMessage = await messageRepo.GetByIdAsync(convDto.LastMessageId, ct);
            
        Conversation? conv = null;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await convRepo.UpdateAsync(request.ConversationId, c =>
            {
                c.Claim(userId);
                c.OnBackOfficeRead(lastMessage.Id);
                conv = c;
            }, ct);

            if (conv != null)
            {
                var convMember = ConversationMember.Create(
                    conv: conv,
                    userId: userId,
                    role: RoleInConversation.Agent);
                await convMemberRepo.AddAsync(convMember, ct);
            }
            await unitOfWork.CommitAsync(ct);
            
            var updatedConv = await convService.GetConvByIdAsync(convDto.ConversationId, ct);
            var convUnread = await convRepo.GetSupportConvUnreadAsync(ct);
            
            await eventDispatcher.Publish(new OnSupportConversationClaimedEvent(updatedConv.Adapt<ConversationSignalDto>()), ct);
            await eventDispatcher.Publish(new OnSupportConversationUnreadEvent(convUnread), ct);
        }, ct);

        return Unit.Value;
    }
}