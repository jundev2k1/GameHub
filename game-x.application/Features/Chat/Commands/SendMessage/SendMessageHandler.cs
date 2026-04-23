using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Chat.OnClientCountTotalUnread;
using game_x.application.Events.Chat.OnDirectMessageCreated;
using game_x.application.Events.Chat.OnPublicMessageCreated;
using game_x.application.Events.Chat.OnSupportConversationUnread;
using game_x.application.Events.Chat.OnSupportMessageCreatedV2;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Commands.SendMessage;

public sealed class SendMessageHandler(
    IUnitOfWork unitOfWork,
    IConversationRepo convRepo,
    IConversationRepo conversationRepo,
    IConversationMemberRepo convMemberRepo,
    IMessageRepo messageRepo,
    IMessageMentionRepo messageMentionRepo,
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
                kind: request.Kind,
                isAgent: request.IsAgent,
                hasAll: request.Mention?.IsAll,
                ct: ct);
            
            await messageService.CreateMessageAttachmentsAsync(msg: message, attachments: request.Attachments, ct);
            
            if(request.Mention != null)
                await BulkCreateMentionsAsync(msg: message, request.Mention, ct);
            
            conv.LastMessageAt = DateTime.UtcNow;
            
            await unitOfWork.SaveChangesAsync(ct);

            if (conv.Type == ConversationType.Direct)
            {
                var member = await convMemberRepo.GetByConvIdAndUserIdAsync(conv.PublicId, actorId, ct);
                if (member != null)
                {
                    await convMemberRepo.UpdateAsync(member.Id, m =>
                    {
                        m.OnRead(message.Id);
                    }, ct);
                }
            }
            
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
        MessageKind kind,
        bool? isAgent,
        bool? hasAll,
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
            kind: kind,
            senderRole: isAgent == true ? RoleInConversation.Agent : RoleInConversation.Member,
            replyToMessageId: replyMessageIntId,
            isMentionAll: conv.Type == ConversationType.Public ? hasAll : null
        );
        
        await messageRepo.AddAsync(msg, ct);
        return msg;
    }
    
    private async Task BulkCreateMentionsAsync(Message msg, MentionRequest mentionRequest, CancellationToken ct)
    {
        IList<MessageMention> mentions = [];
        
        foreach (var item in mentionRequest.Direct ?? [])
        {
            var direct = MessageMention.Create(
                msg: msg,
                userId: item.UserId,
                kind: MentionKind.Direct,
                display: item.Display
            );
            mentions.Add(direct);
        }
        
        if(mentions.Count > 0)
            await messageMentionRepo.BulkAddAsync(mentions, ct);
    }
    
    private async Task SendSignalAsync(SendMessageCommand request, Conversation conv, Message message, CancellationToken ct)
    {
        var dto = await GetMessageDtoAsync(request, message, ct);
        switch (conv.Type)
        {
            case ConversationType.Direct:
            {
                // Count total unread messages when sending friend messages.
                if (conv.Type is ConversationType.Direct)
                {
                    var memberList = await convMemberRepo.GetMembersByConvIdAsync(conv.PublicId, ct);
                    var counterpartId = memberList
                        .Where(x => x.IsHidden != true && x.UserId != request.SenderActorId)
                        .Select(x => x.UserId)
                        .FirstOrDefault();
                    if (counterpartId != null)
                    {
                        var unreadDto =
                            await convMemberRepo.GetTotalUnreadByUserIdAsync(counterpartId, ConversationType.Direct, ct);
                        var totalUnreadCount = unreadDto.FirstOrDefault()?.UnreadCount ?? 0;
                        await eventDispatcher.Publish(new OnClientCountTotalUnreadEvent(counterpartId, totalUnreadCount), ct);
                    }
                }
                
                await eventDispatcher.Publish(new OnDirectMessageCreatedEvent(dto), ct);
                break;
            }
            case ConversationType.Support:
            {
                var convUnread = await convRepo.GetSupportConvUnreadAsync(ct);
                await eventDispatcher.Publish(new OnSupportMessageCreatedV2Event(dto), ct);
                await eventDispatcher.Publish(new OnSupportConversationUnreadEvent(convUnread), ct);
                break;
            }
            case ConversationType.Public:
                await eventDispatcher.Publish(new OnPublicMessageCreatedEvent(dto), ct);
                break;
        }
    }
    
    private async Task<CreatedMessageSignalResult> GetMessageDtoAsync(SendMessageCommand request, Message message, CancellationToken ct)
    {
        var msg = await messageRepo.GetByIdAsync(message.PublicId, ct);
        var updatedConv = await conversationService.GetConvByIdAsync(request.ConversationId, ct);
        
        int? clientUnreadCount = null;
        if (updatedConv.GuestId != null)
            clientUnreadCount = await convRepo.CountConvUnreadByGuestIdAsync(updatedConv.GuestId, updatedConv.ConversationId, ct);
            
        if (updatedConv.CustomerId != null)
            clientUnreadCount = await convRepo.CountSupportConvUnreadByUserIdAsync(updatedConv.CustomerId, updatedConv.ConversationId, ct);
        
        var unreadCount = await convRepo.CountSupportConvUnreadAsync(updatedConv.ConversationId, ct);
        var msgSignalDto = await messageService.GetMessageDtoAsync(msg.Adapt<MessageDto>(), ct);
        return new CreatedMessageSignalResult(
            Msg: msgSignalDto.Adapt<MessageSignalDto>() with {ClientLocalId = request.ClientLocalId},
            Conv: updatedConv.Adapt<ConversationSignalDto>() with {BackOfficeUnreadCount = unreadCount, ClientUnreadCount = clientUnreadCount},
            InboxUpsert: updatedConv.Adapt<InboxUpsertSignalDto>());
    }
}