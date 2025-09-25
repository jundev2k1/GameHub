using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Schedules.Commands.DeleteSchedule;

public sealed class DeleteScheduleHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo) : ICommandHandler<DeleteScheduleCommand>
{
    public async Task<Unit> Handle(DeleteScheduleCommand request, CancellationToken ct = default)
    {
        var targetSetting = await liveStreamRepo.GetByIdAsync(request.Id, ct);
        if (targetSetting.Status != LiveStreamStatus.Scheduled)
            throw new BadRequestException("Only scheduled livestream can be deleted.");

        if (targetSetting.StartTime < DateTime.UtcNow)
            throw new BadRequestException("Cannot delete livestream that has already started.");

        await liveStreamRepo.DeleteAsync(request.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
