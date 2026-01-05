using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Chat.Commands.ClaimConversationById;

public sealed class ClaimConversationByIdHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor, 
    IConversationRepo conversationRepo,
    IConversationMemberRepo conversationMemberRepo) 
    : IRequestHandler<ClaimConversationByIdCommand, Unit>
{
    
    public async Task<Unit> Handle(
        ClaimConversationByIdCommand request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        Conversation? conv = null;
        await unitOfWork.WithTransactionAsync(
            async () =>
            {
                await conversationRepo
                    .UpdateAsync(request.ConversationId, updatedOrder =>
                    {
                        if(updatedOrder.Status != ConversationStatus.Open)
                            throw new BadRequestException(MessageCode.Chatting.ConversationAlreadyClaimed);
                        updatedOrder.Claim(userId);
                        conv = updatedOrder;
                    }, ct);

                if(conv is null)
                    throw new BadRequestException(MessageCode.Chatting.ConversationNotFound);

                var convMember = ConversationMember.Create(
                    conv: conv,
                    userId: userId,
                    role: RoleInConversation.Agent);
        
                await conversationMemberRepo.AddAsync(convMember, ct);
            }, ct);

        return Unit.Value;
    }
}