using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Interactions.Characters.Commands.UpdateCharacter;

public sealed class UpdateCharacterHandler(
    IUnitOfWork unitOfWork,
    IInteractionCharacterRepo characterRepo) : ICommandHandler<UpdateCharacterCommand>
{
    public async Task<Unit> Handle(UpdateCharacterCommand request, CancellationToken ct = default)
    {
        await characterRepo.UpdateAsync(request.Id!.Value, character =>
        {
            character.Update(
                request.Name,
                request.Description,
                request.Notes);
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
