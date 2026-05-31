using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Reminders.Commands.SubscribeStream;

public sealed class SubscribeStreamHandler(
    IUserAccessor userAccessor,
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamReminderRepo streamReminderRepo) : ICommandHandler<SubscribeStreamCommand>
{
    public async Task<Unit> Handle(SubscribeStreamCommand request, CancellationToken ct = default)
    {
        var targetStream = await liveStreamRepo.GetByIdAsync(request.StreamId!.Value, ct);
        if (targetStream.Status != LiveStreamStatus.Scheduled)
            throw new BadRequestException(MessageCode.System.InvalidResourceState);

        var userId = userAccessor.GetUserId();
        var reminders = request.Channels
            .Select(channel => LiveStreamReminder.Create(targetStream.Id, userId, channel));

        await unitOfWork.WithTransactionAsync(async () =>
        {
            await streamReminderRepo.CreateRangeAsync(reminders, ct);
        }, ct);

        return Unit.Value;
    }
}
