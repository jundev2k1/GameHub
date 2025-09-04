using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameTag;

public sealed class UpdateGameTagHandler(
    IUnitOfWork unitOfWork,
    IGameTagRepo gameTagRepo) : ICommandHandler<UpdateGameTagCommand>
{
    public async Task<Unit> Handle(UpdateGameTagCommand request, CancellationToken ct = default)
    {
        await gameTagRepo.UpdateAsync(request.Id, async (tag) =>
        {
            tag.Update(
                request.Name,
                request.Description,
                GameTagIcon.Of(request.Icon),
                GameTagColor.Of(request.Color),
                request.Note);
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        return Unit.Value;
    }
}
