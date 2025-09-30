using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnMarkMessageAsRead;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Commands.MarkMessageAsRead;

public sealed class MarkMessageAsReadHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IConversationMemberRepo convMemberRepo,
    IMessageRepo messageRepo,
    IAppLogger<Message> logger,
    IApplicationEventDispatcher dispatcher
) : IRequestHandler<MarkMessageAsReadCommand, Unit>
{
    public async Task<Unit> Handle(MarkMessageAsReadCommand request, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();

        // Each customer has only one conversation with customer support; if none exists, a new one will be created
        var member = await convMemberRepo.GetByConvIdAndUserIdAsync(request.ConversationId, me, ct)
            ?? throw new BadRequestException(MessageCode.Chatting.IsNotMember);

        var readMessage = await messageRepo.CheckExistByConvIdAsync(request.ConversationId, request.LastReadMessageId, ct)
            ?? throw new NotFoundException(MessageCode.Chatting.MessageNotFound);
        
        if(member.LastReadMessageId != null && readMessage.Id <= member.LastReadMessageId )
            throw new BadRequestException(MessageCode.Chatting.MessageAlreadyRead);
        
        try
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await convMemberRepo.UpdateAsync(member.Id, m =>
                {
                    if (m.LastReadMessageId is null || readMessage.Id > m.LastReadMessageId)
                        m.LastReadMessageId = readMessage.Id;
                    
                    m.LastDeliveredAt = DateTime.UtcNow;
                    m.LastSeenAt = m.LastDeliveredAt;
                }, ct);
                await unitOfWork.CommitAsync(ct);

                var dto = await convMemberRepo.GetUnreadAsync(member.ConversationId, me, ct)
                    ?? new ConvUnreadDto(request.ConversationId, 0);
                await dispatcher.Publish(new OnMarkMessageAsReadEvent(dto, me), ct);
            },ct);
            return Unit.Value;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
}