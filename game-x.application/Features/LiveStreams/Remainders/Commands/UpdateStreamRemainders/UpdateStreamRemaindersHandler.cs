using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Remainders.Commands.UpdateStreamRemainders;

public sealed class UpdateStreamRemaindersHandler(
    IUserAccessor userAccessor,
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamRemainderRepo streamRemainderRepo) : ICommandHandler<UpdateStreamRemaindersCommand>
{
    public async Task<Unit> Handle(UpdateStreamRemaindersCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var schedule = await liveStreamRepo.GetByStreamKeyAsync(request.StreamKey!, ct);
        if (schedule.Status != LiveStreamStatus.Scheduled)
            throw new BadRequestException(MessageCode.System.InvalidResourceState);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            // Delete all channels of user from current stream
            await streamRemainderRepo.DeleteAsync(userId, schedule.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // Create new channels
            if (request.Channels.Length > 0)
            {
                var remainders = request.Channels
                    .Select(channel => LiveStreamReminder.Create(schedule.Id, userId, channel))
                    .ToArray();
                await streamRemainderRepo.CreateRangeAsync(remainders, ct);
            }
        }, ct);

        return Unit.Value;
    }
}
