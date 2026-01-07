using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnMarkMessageAsRead;
using game_x.application.Events.OnSupportConversationUnread;

namespace game_x.application.Features.Chat.Commands.MarkMessageAsRead;

public sealed class MarkMessageAsReadHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IConversationRepo convRepo,
    IConversationMemberRepo convMemberRepo,
    IConversationService conversationService,
    IMessageRepo messageRepo,
    IAppLogger<Message> logger,
    IApplicationEventDispatcher dispatcher
) : IRequestHandler<MarkMessageAsReadCommand, Unit>
{
    public async Task<Unit> Handle(MarkMessageAsReadCommand cmd, CancellationToken ct)
    {
        var readMessage = await messageRepo.CheckExistByConvIdAsync(cmd.ConversationId, cmd.LastReadMessageId, ct)
            ?? throw new NotFoundException(MessageCode.Chatting.MessageNotFound);

        if (cmd.GuestId != null)
        {
            await MarkAsReadForGuest(cmd, readMessage, ct);
            return Unit.Value;
        }
        
        var me = userAccessor.GetUserId();
        var role = userAccessor.GetRoles();
        
        if (role.IsUser) await MarkAsReadForClient(cmd, readMessage, ct);
        if (role.IsBackOffice) await MarkAsReadForBackOffice(me, role, readMessage, cmd, ct);
        return Unit.Value;
    }

    private async Task MarkAsReadForClient(
        MarkMessageAsReadCommand cmd,
        Message readMessage,
        CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var role = userAccessor.GetRoles();
        
        // Each customer has only one conversation with customer support; if none exists, a new one will be created
        var member = await convMemberRepo.GetByConvIdAndUserIdAsync(cmd.ConversationId, userId, ct)
                     ?? throw new BadRequestException(MessageCode.Chatting.IsNotMember);
        
        if(member.LastReadMessageId != null && readMessage.Id <= member.LastReadMessageId)
            return;
        
        try
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await convMemberRepo.UpdateAsync(member.Id, m =>
                {
                    m.OnRead(readMessage.Id);
                }, ct);
                await unitOfWork.CommitAsync(ct);

                var updatedConv = await conversationService.GetConvByIdAsync(cmd.ConversationId, ct);
                int? clientUnreadCount = updatedConv.CustomerId != null
                    ? await convRepo.CountConvUnreadByUserIdAsync(updatedConv.CustomerId, updatedConv.ConversationId, ct)
                    : null;
                
                var dto = updatedConv.Adapt<ConversationSignalDto>() with {ClientUnreadCount = clientUnreadCount};
                await dispatcher.Publish(new OnMarkMessageAsReadEvent(dto, userId, role), ct);
            },ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
    
    private async Task MarkAsReadForBackOffice(
        string userId,
        AppRole role,
        Message readMessage,
        MarkMessageAsReadCommand cmd,
        CancellationToken ct)
    {
        var conv = await convRepo.GetByIdAsync(cmd.ConversationId, ct);
        if (conv.Type != ConversationType.Support)
            throw new BadRequestException(MessageCode.Chatting.ConversationNotFound);
        
        if(conv.LastResolvedMessageId != null && readMessage.Id <= conv.LastResolvedMessageId)
            return;
        try
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await convRepo.UpdateAsync(conv.PublicId, c =>
                {
                    c.OnBackOfficeRead(readMessage.Id);
                }, ct);
                await unitOfWork.CommitAsync(ct);
                
                var updatedConv = await conversationService.GetConvByIdAsync(conv.PublicId, ct);
                var unreadCount = await convRepo.CountSupportConvUnreadAsync(updatedConv.ConversationId, ct);
                var dto = updatedConv.Adapt<ConversationSignalDto>() with {BackOfficeUnreadCount = unreadCount};
                
                var convUnread = await convRepo.GetSupportConvUnreadAsync(ct);
                await dispatcher.Publish(new OnMarkMessageAsReadEvent(dto, userId, role), ct);
                await dispatcher.Publish(new OnSupportConversationUnreadEvent(convUnread), ct);
            },ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
    
    private async Task MarkAsReadForGuest(
        MarkMessageAsReadCommand cmd,
        Message readMessage,
        CancellationToken ct)
    {
        var conv = await convRepo.GetByIdAsync(cmd.ConversationId, ct);
        if (conv.Type != ConversationType.Support)
            throw new BadRequestException(MessageCode.Chatting.ConversationNotFound);
        
        if(conv.LastGuestReadMessageId != null && readMessage.Id <= conv.LastGuestReadMessageId)
            return;
        
        try
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await convRepo.UpdateAsync(conv.PublicId, c => { c.OnGuestRead(readMessage.Id); }, ct);
                await unitOfWork.CommitAsync(ct);

                var updatedConv = await conversationService.GetConvByIdAsync(cmd.ConversationId, ct);
                int? clientUnreadCount = updatedConv.GuestId != null
                    ? await convRepo.CountConvUnreadByGuestIdAsync(updatedConv.GuestId, updatedConv.ConversationId, ct)
                    : null;
                
                var dto = updatedConv.Adapt<ConversationSignalDto>() with {ClientUnreadCount = clientUnreadCount};
                if (cmd.GuestId != null)
                    await dispatcher.Publish(new OnMarkMessageAsReadEvent(dto, cmd.GuestId, AppRole.Of(AppRoles.Guest)), ct);
            },ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
}