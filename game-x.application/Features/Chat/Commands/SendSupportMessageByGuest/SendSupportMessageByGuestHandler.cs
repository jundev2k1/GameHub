using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Chat.Commands.SendSupportMessageByGuest;

public sealed class SendSupportMessageByGuestHandler(
    IUnitOfWork unitOfWork,
    IConversationRepo conversationRepo,
    IMessageRepo messageRepo,
    IAppLogger<Message> logger
    ) : IRequestHandler<SendSupportMessageByGuestCommand, SendSupportMessageByGuestResult>
{
    public async Task<SendSupportMessageByGuestResult> Handle(SendSupportMessageByGuestCommand request, CancellationToken ct)
    {
        var senderUserId = request.GuestId;
        var now = DateTime.UtcNow;

        // Each guest has only one conversation with customer support; if none exists, a new one will be created
        var conv = await conversationRepo.GetSupportConversationForGuestAsync(senderUserId, ct);
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            if (conv is null)
            {
                conv = await CreateConversationAsync(senderUserId, ct);
            }
            
            var message = await CreateMessageAsync(conv, senderUserId, request.Text, ct);
            
            conv.LastMessageAt = now;

            await unitOfWork.CommitAsync(ct);
            
            return new SendSupportMessageByGuestResult(
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
    
    private async Task<Conversation> CreateConversationAsync(string guestId, CancellationToken ct)
    {
        var conv = Conversation.Create(
            type: ConversationType.Support,
            senderGuestId: guestId);
                
        await conversationRepo.AddAsync(conv, ct);
        return conv;
    }
    
    private async Task<Message> CreateMessageAsync(Conversation conv, string userId, string text, CancellationToken ct)
    {
        var msg = Message.Create(
            conv: conv,
            senderActorId: userId,
            text: text,
            kind: MessageKind.Text,
            senderRole: RoleInConversation.Member
        );
        
        await messageRepo.AddAsync(msg, ct);
        return msg;
    }
}