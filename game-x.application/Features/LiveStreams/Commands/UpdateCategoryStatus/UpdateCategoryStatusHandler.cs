using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Commands.UpdateCategoryStatus;

public sealed class UpdateCategoryStatusHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamCategoryRepo liveStreamCategoryRepo) : ICommandHandler<UpdateCategoryStatusCommand>
{
    public async Task<Unit> Handle(UpdateCategoryStatusCommand request, CancellationToken ct = default)
    {
        await liveStreamCategoryRepo.UpdateAsync(request.Id, async category =>
        {
            category.UpdateStatus(request.IsActive);
            await Task.CompletedTask;
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
