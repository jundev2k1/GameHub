using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnSupportMessageCreated;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Commands.SendMessageToCustomer;

public sealed class SendMessageToCustomerHandler(
    IUnitOfWork unitOfWork,
    IConversationRepo conversationRepo,
    IConversationService conversationService,
    IMessageRepo messageRepo,
    IMessageService messageService,
    IAppLogger<Message> logger,
    IUserAccessor userAccessor,
    IApplicationEventDispatcher eventDispatcher
    ) : IRequestHandler<SendMessageToCustomerCommand, SendMessageToCustomerResult>
{
    public async Task<SendMessageToCustomerResult> Handle(SendMessageToCustomerCommand request, CancellationToken ct)
    {
        var senderUserId = userAccessor.GetUserId();
        var now = DateTime.UtcNow;

        var conv = await conversationRepo.GetByIdAsync(request.ConversationId, ct);
        
        // The conversation needs to be claimed before sending a message
        if(conv.Status != ConversationStatus.Claimed)
            throw new BadRequestException(MessageCode.Chatting.ConversationNotClaimed);
        
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var message = await CreateMessageAsync(
                conv: conv, 
                userId: senderUserId,
                text: request.Text ?? string.Empty,
                replyMessageId: request.ReplyToMessageId,
                hasAttachment: request.Attachments?.Count > 0,
                ct: ct);
            
            conv.LastMessageAt = now;

            await messageService.CreateMessageAttachmentsAsync(msg: message, attachments: request.Attachments, ct);
            
            await unitOfWork.CommitAsync(ct);
            
            var msgDto = new MessageDto
            {
                Id = message.Id,
                PublicId = message.PublicId,
                ConversationId = conv.PublicId,
                SenderActorId = message.SenderActorId,
                SenderRole = message.SenderRole,
                Kind = message.Kind,
                Text = message.Text,
                ReplyToMessageId = request.ReplyToMessageId,
                IsTombstone = message.IsTombstone,
                SentAt = message.SentAt,
                EditedAt = message.EditedAt,
                EditCount = message.EditCount,
                CurrentVersion = message.CurrentVersion,
                Attachments = message.Attachments.Adapt<List<MessageAttachmentDto>>()
            };
            
            var updatedConv = await conversationService.GetConvByIdAsync(conv.PublicId, ct);
            var msgSignalDto = await messageService.GetMessageDtoAsync(msgDto, ct);
            var dto = new CreatedMessageSignalResult(
                Msg: msgSignalDto.Adapt<MessageSignalDto>() with {ClientLocalId = request.ClientLocalId},
                Conv: updatedConv.Adapt<ConversationSignalDto>(),
                InboxUpsert: updatedConv.Adapt<InboxUpsertSignalDto>());
            
            await eventDispatcher.Publish(new OnSupportMessageCreatedEvent(dto), ct);
            
            return new SendMessageToCustomerResult(request.ClientLocalId, conv.PublicId);
        }
        catch(Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError("Failed to create the message: {Ex}", ex);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
    
    private async Task<Message> CreateMessageAsync(
        Conversation conv, 
        string userId, 
        string text,
        Guid? replyMessageId,
        bool hasAttachment,
        CancellationToken ct)
    {
        int? replyMessageIntId = null;
        if (replyMessageId is not null && replyMessageId.Value != Guid.Empty)
        {
            var rId = replyMessageId.Value;
            var replyMessage = await messageRepo.CheckExistAsync(rId, ct);
            replyMessageIntId = replyMessage.Id;
        }
        
        var msg = Message.Create(
            conv: conv,
            senderActorId: userId,
            senderUserId: userId,
            text: text,
            kind: hasAttachment ? MessageKind.Attachment : MessageKind.Text,
            senderRole: RoleInConversation.Agent,
            replyToMessageId: replyMessageIntId
        );
        await messageRepo.AddAsync(msg, ct);
        return msg;
    }
}