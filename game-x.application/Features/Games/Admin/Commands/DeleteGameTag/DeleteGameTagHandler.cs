using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.DeleteGameTag;

public sealed class DeleteGameTagHandler(
    IUnitOfWork unitOfWork,
    IGameTagRepo gameTagRepo) : ICommandHandler<DeleteGameTagCommand>
{
    public async Task<Unit> Handle(DeleteGameTagCommand request, CancellationToken ct = default)
    {
        await gameTagRepo.DeleteAsync(request.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
