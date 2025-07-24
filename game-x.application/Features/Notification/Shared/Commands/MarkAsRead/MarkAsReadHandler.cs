using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Notification.Shared.Commands.MarkAsRead;

public sealed class MarkAsReadHandler(INotificationRepo notificationRepo, IUnitOfWork unitOfWork)
    : ICommandHandler<MarkAsReadCommand>
{
    public async Task<Unit> Handle(MarkAsReadCommand command, CancellationToken ct = default)
    {
        await notificationRepo.MarkAsReadAsync(command.NotificationId, command.UserId, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
