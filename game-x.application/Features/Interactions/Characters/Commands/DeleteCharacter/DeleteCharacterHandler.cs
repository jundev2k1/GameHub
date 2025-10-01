using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Interactions.Characters.Commands.DeleteCharacter;

public sealed class DeleteCharacterHandler(
    IUnitOfWork unitOfWork,
    IInteractionCharacterRepo characterRepo) : ICommandHandler<DeleteCharacterCommand>
{
    public async Task<Unit> Handle(DeleteCharacterCommand request, CancellationToken ct = default)
    {
        await characterRepo.DeleteAsync(request.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
