using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Commands.UpdateCategory;

public sealed class UpdateCategoryHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamCategoryRepo liveStreamCategoryRepo) : ICommandHandler<UpdateCategoryCommand>
{
    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken ct = default)
    {
        await liveStreamCategoryRepo.UpdateAsync(request.Id, async category =>
        {
            category.Update(
                request.Name,
                request.Description,
                request.Note,
                request.Priority,
                request.IsActive);
            await Task.CompletedTask;
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
