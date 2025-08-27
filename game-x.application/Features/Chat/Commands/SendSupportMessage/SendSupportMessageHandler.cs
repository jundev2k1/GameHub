using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Chat.Commands.SendSupportMessage;

public sealed class SendSupportMessageHandler(
    IUnitOfWork unitOfWork,
    IConversationRepo conversationRepo,
    IConversationMemberRepo conversationMemberRepo,
    IMessageRepo messageRepo,
    IAppLogger<Message> logger,
    IUserAccessor userAccessor
    ) : IRequestHandler<SendSupportMessageCommand, SendSupportMessageResult>
{
    public async Task<SendSupportMessageResult> Handle(SendSupportMessageCommand request, CancellationToken ct)
    {
        var senderUserId = userAccessor.GetUserId();
        var now = DateTime.UtcNow;

        // Each customer has only one conversation with customer support; if none exists, a new one will be created
        var conv = await conversationRepo.GetSupportConversationAsync(ConversationStatus.Claimed, senderUserId, ct);
        ConversationMember? convMember = null;
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            if (conv is null)
            {
                conv = await CreateConversationAsync(senderUserId, ct);
                convMember = await CreateConversationMemberAsync(conv, senderUserId, ct);
            }

            if (convMember is null)
            {
                var exists = await conversationMemberRepo.CheckExistMemberAsync(conv.Id, senderUserId, ct);
                if (!exists)
                {
                    await CreateConversationMemberAsync(conv, senderUserId, ct);
                }
            }
            
            var message = await CreateMessageAsync(conv, senderUserId, request.Text, ct);
            
            conv.LastMessageAt = now;

            await unitOfWork.CommitAsync(ct);
            
            return new SendSupportMessageResult(
                message.Adapt<MessageDto>(), 
                conv.Adapt<ConversationDto>());
            
        }
        catch(Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError("Failed to create the message: {Ex}", ex);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
    
    private async Task<Conversation> CreateConversationAsync(string userId, CancellationToken ct)
    {
        var conv = Conversation.Create(
            type: ConversationType.Support,
            senderUserId: userId);
                
        await conversationRepo.AddAsync(conv, ct);
        return conv;
    }
    
    private async Task<ConversationMember> CreateConversationMemberAsync(Conversation conv, string userId, CancellationToken ct)
    {
        var convMember = ConversationMember.Create(
                conv: conv,
                userId: userId,
                role: RoleInConversation.Member);
        
        await conversationMemberRepo.AddAsync(convMember, ct);
        return convMember;
    }
    
    private async Task<Message> CreateMessageAsync(Conversation conv, string userId, string text, CancellationToken ct)
    {
        var msg = new Message
        {
            Conversation = conv,
            SenderUserId = userId,
            SenderRole = RoleInConversation.Member,
            Kind = MessageKind.Text,
            Text = text,
            SentAt = DateTime.UtcNow,
            IsTombstone = false,
            EditCount = 0,
            CurrentVersion = 1,
        };
        
        await messageRepo.AddAsync(msg, ct);
        return msg;
    }
}