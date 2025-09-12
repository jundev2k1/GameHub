using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Commands.DeleteCategory;

public sealed class DeleteCategoryHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamCategoryRepo liveStreamCategoryRepo) : ICommandHandler<DeleteCategoryCommand>
{
    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken ct = default)
    {
        await liveStreamCategoryRepo.DeleteAsync(request.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
