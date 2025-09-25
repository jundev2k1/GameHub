using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnDirectMessageCreated;
using game_x.application.Events.OnPublicMessageCreated;
using game_x.application.Events.OnSupportMessageCreatedV2;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Commands.SendMessage;

public sealed class SendMessageHandler(
    IUnitOfWork unitOfWork,
    IConversationRepo conversationRepo,
    IMessageRepo messageRepo,
    IMessageService messageService,
    IConversationService conversationService,
    IAppLogger<Message> logger,
    IApplicationEventDispatcher eventDispatcher
    ) : IRequestHandler<SendMessageCommand, SendMessageResult>
{
    public async Task<SendMessageResult> Handle(SendMessageCommand request, CancellationToken ct)
    {
        var actorId = request.SenderActorId;
        var userId = request.SenderUserId;

        var conv = await conversationRepo.GetByIdAndActorIdAsync(actorId, request.ConversationId, ct);

        if(conv.Status == ConversationStatus.Closed)
            throw new BadRequestException(MessageCode.Chatting.ConversationClosed);

        // The conversation needs to be claimed before sending a message
        if(request.IsAgent == true && conv.Status != ConversationStatus.Claimed)
            throw new BadRequestException(MessageCode.Chatting.ConversationNotClaimed);

        // Only members can send messages to Public Conversation
        if(conv.Type == ConversationType.Public && (userId == null || request.IsAgent == true))
            throw new BadRequestException(MessageCode.System.Forbidden);
            
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var message = await CreateMessageAsync(
                conv: conv,
                actorId: actorId,
                userId: userId,
                text: request.Text,
                replyMessageId: request.ReplyToMessageId,
                hasAttachment: request.Attachments?.Count > 0,
                isAgent: request.IsAgent,
                ct: ct);
            
            await messageService.CreateMessageAttachmentsAsync(msg: message, attachments: request.Attachments, ct);
            
            conv.LastMessageAt = DateTime.UtcNow;
            
            await unitOfWork.CommitAsync(ct);

            await SendSignalAsync(request, conv, message, ct);
           
            return new SendMessageResult(request.ClientLocalId, request.ConversationId);
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
        string actorId,
        string? userId,
        string? text,
        Guid? replyMessageId,
        bool hasAttachment,
        bool? isAgent,
        CancellationToken ct)
    {
        int? replyMessageIntId = null;
        if (replyMessageId is not null && replyMessageId.Value != Guid.Empty)
        {
            var rid = replyMessageId.Value;
            var replyMessage = await messageRepo.GetByIdAsync(rid, ct);
            replyMessageIntId = replyMessage.Id;
        }
        
        var msg = Message.Create(
            conv: conv,
            senderActorId: actorId,
            senderUserId: userId,
            text: text,
            kind: hasAttachment ? MessageKind.Attachment : MessageKind.Text,
            senderRole: isAgent == true ? RoleInConversation.Agent : RoleInConversation.Member,
            replyToMessageId: replyMessageIntId
        );
        
        await messageRepo.AddAsync(msg, ct);
        return msg;
    }
    
    private async Task SendSignalAsync(SendMessageCommand request, Conversation conv, Message message, CancellationToken ct)
    {
        var dto = await GetMessageDtoAsync(request, message, ct);
        switch (conv.Type)
        {
            case ConversationType.Direct:
                await eventDispatcher.Publish(new OnDirectMessageCreatedEvent(dto), ct);
                break;
            case ConversationType.Support:
                await eventDispatcher.Publish(new OnSupportMessageCreatedV2Event(dto), ct);
                break;
            case ConversationType.Public:
                await eventDispatcher.Publish(new OnPublicMessageCreatedEvent(dto), ct);
                break;
        }
    }
    
    private async Task<CreatedMessageSignalResult> GetMessageDtoAsync(SendMessageCommand request, Message message, CancellationToken ct)
    {
        var msgDto = new MessageDto
        {
            Id = message.Id,
            PublicId = message.PublicId,
            ConversationId = request.ConversationId,
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
        
        var updatedConv = await conversationService.GetConversationDetailAsync(request.ConversationId, ct);
        var msgSignalDto = await messageService.GetMessageDtoAsync(msgDto, ct);
        return new CreatedMessageSignalResult(
            Msg: msgSignalDto.Adapt<MessageSignalDto>() with {ClientLocalId = request.ClientLocalId},
            Conv: updatedConv.Adapt<ConversationSignalDto>(),
            InboxUpsert: updatedConv.Adapt<InboxUpsertSignalDto>());
    }
}