using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Notifications.Shared.Commands.MarkAllAsRead;

public sealed class MarkAllAsReadHandler(INotificationRepo notificationRepo, IUnitOfWork unitOfWork)
    : ICommandHandler<MarkAllAsReadCommand>
{
    public async Task<Unit> Handle(MarkAllAsReadCommand command, CancellationToken ct = default)
    {
        await notificationRepo.MarkAllAsReadAsync(command.UserId, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
