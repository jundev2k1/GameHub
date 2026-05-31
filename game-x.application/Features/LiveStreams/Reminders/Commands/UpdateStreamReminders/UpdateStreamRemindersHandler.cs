using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Reminders.Commands.UpdateStreamReminders;

public sealed class UpdateStreamRemindersHandler(
    IUserAccessor userAccessor,
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamReminderRepo streamRemainderRepo) : ICommandHandler<UpdateStreamRemindersCommand>
{
    public async Task<Unit> Handle(UpdateStreamRemindersCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var schedule = await liveStreamRepo.GetByIdAsync(request.StreamId!.Value, ct);
        if (schedule.Status != LiveStreamStatus.Scheduled)
            throw new BadRequestException(MessageCode.System.InvalidResourceState);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            // Delete all channels of user from current stream
            await streamRemainderRepo.DeleteAsync(userId, schedule.Id, ct);

            // Create new channels
            if (request.Channels.Length > 0)
            {
                var reminders = request.Channels
                    .Select(channel => LiveStreamReminder.Create(schedule.Id, userId, channel))
                    .ToArray();
                await streamRemainderRepo.CreateRangeAsync(reminders, ct);
            }
        }, ct);

        return Unit.Value;
    }
}
