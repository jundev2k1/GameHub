using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Categories.Commands.CreateCategory;

public sealed class CreateCategoryHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamCategoryRepo liveStreamCategoryRepo) : ICommandHandler<CreateCategoryCommand>
{
    public async Task<Unit> Handle(CreateCategoryCommand request, CancellationToken ct = default)
    {
        var categoryEntity = LiveStreamCategory.Create(
            request.Name,
            request.Description,
            request.Note,
            request.Priority,
            request.IsActive);
        await liveStreamCategoryRepo.CreateAsync(categoryEntity, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
