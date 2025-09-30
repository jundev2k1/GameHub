using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Chat.Commands.HideToggleConversation;

public sealed class HideToggleConversationHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IConversationRepo conversationRepo,
    IConversationMemberRepo convMemberRepo,
    ILogger<ConversationMember> logger
) : IRequestHandler<HideToggleConversationCommand, Unit>
{
    public async Task<Unit> Handle(HideToggleConversationCommand request, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();

        var conversation = await conversationRepo.GetByIdAndActorIdAsync(me, request.ConversationId, ct);
        
        if (conversation.Type == ConversationType.Support)
            throw new BadRequestException(MessageCode.System.UnsupportedType);
        
        var member = await convMemberRepo.GetByConvIdAndUserIdAsync(conversation.PublicId, me, ct)
            ?? throw new NotFoundException(MessageCode.Chatting.IsNotMember);

        if (request.IsHidden == (member.IsHidden ?? false))
            throw new BadRequestException(MessageCode.System.InvalidParameters);
        try
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await convMemberRepo.UpdateAsync(member.Id, m =>
                {
                    m.IsHidden = request.IsHidden;
                }, ct);
                await unitOfWork.CommitAsync(ct);
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