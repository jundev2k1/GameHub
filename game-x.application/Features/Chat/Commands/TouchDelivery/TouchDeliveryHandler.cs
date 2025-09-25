using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Chat.Commands.TouchDelivery;

public sealed class TouchDeliveryHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IConversationMemberRepo convMemberRepo,
    IAppLogger<Message> logger
) : IRequestHandler<TouchDeliveryCommand, Unit>
{
    public async Task<Unit> Handle(TouchDeliveryCommand request, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();

        // Each customer has only one conversation with customer support; if none exists, a new one will be created
        var member = await convMemberRepo.GetByConvIdAndUserIdAsync(request.ConversationId, me, ct);
        if (member is null)
            throw new BadRequestException(MessageCode.Chatting.IsNotMember);

        try
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await convMemberRepo.UpdateAsync(member.Id, m =>
                {
                    m.LastDeliveredAt = DateTime.UtcNow;
                }, ct);
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