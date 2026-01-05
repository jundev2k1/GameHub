using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnMarkMessageAsRead;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Commands.MarkMessageAsRead;

public sealed class MarkMessageAsReadHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IConversationRepo convRepo,
    IConversationMemberRepo convMemberRepo,
    IMessageRepo messageRepo,
    IAppLogger<Message> logger,
    IApplicationEventDispatcher dispatcher
) : IRequestHandler<MarkMessageAsReadCommand, Unit>
{
    public async Task<Unit> Handle(MarkMessageAsReadCommand cmd, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();
        var role = userAccessor.GetRoles();
        
        var readMessage = await messageRepo.CheckExistByConvIdAsync(cmd.ConversationId, cmd.LastReadMessageId, ct)
            ?? throw new NotFoundException(MessageCode.Chatting.MessageNotFound);
        
        if (role.IsUser) await MarkAsReadForClient(me, role, cmd, readMessage, ct);
        if (role.IsBackOffice) await MarkAsReadForBackOffice(me, role, readMessage, cmd, ct);
        return Unit.Value;
    }

    private async Task MarkAsReadForClient(
        string userId,
        AppRole role,
        MarkMessageAsReadCommand cmd,
        Message readMessage,
        CancellationToken ct)
    {
        // Each customer has only one conversation with customer support; if none exists, a new one will be created
        var member = await convMemberRepo.GetByConvIdAndUserIdAsync(cmd.ConversationId, userId, ct)
                     ?? throw new BadRequestException(MessageCode.Chatting.IsNotMember);
        
        if(member.LastReadMessageId != null && readMessage.Id <= member.LastReadMessageId)
            throw new BadRequestException(MessageCode.Chatting.MessageAlreadyRead);
        
        try
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await convMemberRepo.UpdateAsync(member.Id, m =>
                {
                    m.OnRead(readMessage.Id);
                }, ct);
                await unitOfWork.CommitAsync(ct);

                var dto = await convMemberRepo.GetUnreadAsync(member.ConversationId, userId, ct)
                          ?? new ConvUnreadDto(cmd.ConversationId, 0);
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
        
        try
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await convRepo.UpdateAsync(conv.PublicId, c =>
                {
                    c.OnBackOfficeRead(readMessage.Id);
                }, ct);
                await unitOfWork.CommitAsync(ct);

                var unreadMessage = await convRepo.CountSupportConvUnreadAsync(conv.PublicId, ct);
                var dto = new ConvUnreadDto(cmd.ConversationId, unreadMessage);
                await dispatcher.Publish(new OnMarkMessageAsReadEvent(dto, userId, role), ct);
            },ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
}