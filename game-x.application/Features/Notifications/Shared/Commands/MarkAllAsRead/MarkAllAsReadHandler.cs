using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Notifications.Shared.Commands.MarkAllAsRead;

public sealed class MarkAllAsReadHandler(INotificationRepo notificationRepo)
    : ICommandHandler<MarkAllAsReadCommand>
{
    public async Task<Unit> Handle(MarkAllAsReadCommand command, CancellationToken ct = default)
    {
        await notificationRepo.MarkAllAsReadAsync(command.UserId, ct);
        return Unit.Value;
    }
}
