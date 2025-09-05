using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Chat.Commands.SendMessageToCustomer;

public sealed class SendMessageToCustomerHandler(
    IUnitOfWork unitOfWork,
    IConversationRepo conversationRepo,
    IMessageRepo messageRepo,
    IAppLogger<Message> logger,
    IUserAccessor userAccessor
    ) : IRequestHandler<SendMessageToCustomerCommand, SendMessageToCustomerResult>
{
    public async Task<SendMessageToCustomerResult> Handle(SendMessageToCustomerCommand request, CancellationToken ct)
    {
        var senderUserId = userAccessor.GetUserId();
        var now = DateTime.UtcNow;

        var conv = await conversationRepo.GetByIdAsync(request.ConversationId, ct);
        
        if(conv.Status != ConversationStatus.Claimed)
            throw new BadRequestException(MessageCode.Chatting.ConversationNotClaimed);
        
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var message = await CreateMessageAsync(conv, senderUserId, request.Text, ct);
            
            conv.LastMessageAt = now;

            await unitOfWork.CommitAsync(ct);
            
            return new SendMessageToCustomerResult(
                message.Adapt<MessageDto>(), 
                conv.Adapt<ConversationSignalDto>());
        }
        catch(Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError("Failed to create the message: {Ex}", ex);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
    
    private async Task<Message> CreateMessageAsync(Conversation conv, string userId, string text, CancellationToken ct)
    {
        var msg = Message.Create(
            conv: conv,
            senderActorId: userId,
            senderUserId: userId,
            text: text,
            kind: MessageKind.Text,
            senderRole: RoleInConversation.Agent
        );
        await messageRepo.AddAsync(msg, ct);
        return msg;
    }
}