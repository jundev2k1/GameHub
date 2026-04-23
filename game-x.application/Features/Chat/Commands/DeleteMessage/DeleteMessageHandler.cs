using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Chat.OnDeleteMessage;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Commands.DeleteMessage;

public sealed class DeleteMessageHandler(
    IUnitOfWork unitOfWork,
    IMessageRepo messageRepo,
    IConversationRepo convRepo,
    IAppLogger<Message> logger,
    IApplicationEventDispatcher dispatcher) : IRequestHandler<DeleteMessageCommand, Unit>
{
    public async Task<Unit> Handle(DeleteMessageCommand request, CancellationToken ct)
    {
        var targetedConv = await convRepo.GetByIdAsync(request.ConversationId, ct);
        var targetedMessage = await messageRepo.CheckExistAsync(request.MessageId, ct);
        try
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await messageRepo.UpdateAsync(targetedMessage.PublicId, m => { m.Remove(); }, ct);
                await unitOfWork.CommitAsync(ct);

                var dto = new DeletedMessageDto(targetedMessage.PublicId, targetedConv.PublicId, targetedConv.Type);
                await dispatcher.Publish(new OnDeleteMessageEvent(dto), ct);
            }, ct);
            return Unit.Value;
        }
        catch (NotFoundException ex)
        {
            logger.LogError(ex.Message, ex);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
            throw new BadRequestException(MessageCode.System.SystemError, new { ex.Message });
        }
    }
}