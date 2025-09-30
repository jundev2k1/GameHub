using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Commands.MarkMessageAsRead;

namespace game_x.application.Features.Chat.Commands.MarkLatestMessageAsRead;

public sealed class MarkLatestMessageAsReadHandler(
    IUserAccessor userAccessor,
    IConversationRepo conversationRepo,
    IMessageRepo messageRepo,
    ISender sender
) : IRequestHandler<MarkLatestMessageAsReadCommand, Guid>
{
    public async Task<Guid> Handle(MarkLatestMessageAsReadCommand request, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();

        var conversation = await conversationRepo.GetByIdAndActorIdAsync(me,  request.ConversationId, ct);
        var lastedMessage = await messageRepo.GetLastedMessageAsync(conversation.Id, ct);

        if (lastedMessage == null)
            throw new NotFoundException(MessageCode.Chatting.NoMessageFound);
        await sender.Send(new MarkMessageAsReadCommand(conversation.PublicId, lastedMessage.PublicId), ct);
   
        return lastedMessage.PublicId;
    }
}