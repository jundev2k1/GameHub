using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Commands.CancelSchedule;

public sealed class CancelScheduleHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo) : ICommandHandler<CancelScheduleCommand>
{
    public async Task<Unit> Handle(CancelScheduleCommand request, CancellationToken ct = default)
    {
        await liveStreamRepo.UpdateAsync(request.Id, async liveStream =>
        {
            liveStream.CancelStream(request.Reason);
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);
        return Unit.Value;
    }
}
