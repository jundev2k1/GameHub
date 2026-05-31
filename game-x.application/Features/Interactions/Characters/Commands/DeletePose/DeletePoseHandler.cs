using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Interactions.Characters.Commands.DeletePose;

public sealed class DeletePoseHandler(
    IUnitOfWork unitOfWork,
    IInteractionCharacterRepo characterRepo) : ICommandHandler<DeletePoseCommand>
{
    public async Task<Unit> Handle(DeletePoseCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await characterRepo.DeletePoseAsync(request.Id, ct);
        }, ct);

        return Unit.Value;
    }
}
