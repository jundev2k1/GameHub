using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnSupportMessageCreated;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Commands.SendSupportMessage;

public sealed class SendSupportMessageHandler(
    IUnitOfWork unitOfWork,
    IConversationRepo conversationRepo,
    IConversationMemberRepo conversationMemberRepo,
    IMessageRepo messageRepo,
    IMessageService messageService,
    IConversationService conversationService,
    IAppLogger<Message> logger,
    IApplicationEventDispatcher eventDispatcher
    ) : IRequestHandler<SendSupportMessageCommand, SendSupportMessageResult>
{
    public async Task<SendSupportMessageResult> Handle(SendSupportMessageCommand request, CancellationToken ct)
    {
        var senderActorId = request.SenderActorId;
        var senderUserId = request.SenderUserId;
        var now = DateTime.UtcNow;

        // Each customer has only one conversation with customer support; if none exists, a new one will be created
        var conv = await conversationRepo.GetSupportConversationAsync(senderActorId, ct);
        // Only user need join into a group (guest is not allowed)
        ConversationMember? convMember = null;
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            if (conv is null)
            {
                conv = await CreateConversationAsync(senderActorId, senderUserId, ct);
                if (senderUserId is not null)
                {
                    convMember = await CreateConversationMemberAsync(conv, senderUserId, ct);
                }
            }

            if (convMember is null && senderUserId is not null)
            {
                var exists = await conversationMemberRepo.CheckExistMemberAsync(conv.Id, senderUserId, ct);
                if (!exists)
                {
                    await CreateConversationMemberAsync(conv, senderActorId, ct);
                }
            }
            
            var message = await CreateMessageAsync(
                conv: conv, 
                actorId: senderActorId,
                userId: senderUserId,
                text: request.Text,
                replyMessageId: request.ReplyToMessageId,
                hasAttachment: request.Attachments?.Count > 0,
                ct: ct);
            
            await messageService.CreateMessageAttachmentsAsync(msg: message, attachments: request.Attachments, ct);
            
            conv.LastMessageAt = now;
            
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
            
            return new SendSupportMessageResult(request.ClientLocalId);
        }
        catch(Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError("Failed to create the message: {Ex}", ex);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
    
    private async Task<Conversation> CreateConversationAsync(string actorId, string? userId, CancellationToken ct)
    {
        var conv = Conversation.Create(
            type: ConversationType.Support,
            senderUserId: userId,
            senderGuestId: userId is null ? actorId : null);
                
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
    
    private async Task<Message> CreateMessageAsync(
        Conversation conv, 
        string actorId,
        string? userId,
        string? text,
        Guid? replyMessageId,
        bool hasAttachment,
        CancellationToken ct)
    {
        int? replyMessageIntId = null;
        if (replyMessageId is not null && replyMessageId.Value != Guid.Empty)
        {
            var rid = replyMessageId.Value;
            var replyMessage = await messageRepo.CheckExistAsync(rid, ct);
            replyMessageIntId = replyMessage.Id;
        }
        
        var msg = Message.Create(
            conv: conv,
            senderActorId: actorId,
            senderUserId: userId,
            text: text,
            kind: hasAttachment ? MessageKind.Attachment : MessageKind.Text,
            senderRole: RoleInConversation.Member,
            replyToMessageId: replyMessageIntId
        );
        
        await messageRepo.AddAsync(msg, ct);
        return msg;
    }
}