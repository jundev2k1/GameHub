using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.S2s.Commands.DeleteS2sClient;

public sealed class DeleteS2sClientHandler(
    IUnitOfWork unitOfWork,
    IS2sClientRepo s2SClientRepo) : ICommandHandler<DeleteS2sClientCommand>
{
    public async Task<Unit> Handle(DeleteS2sClientCommand request, CancellationToken ct = default)
    {

        await unitOfWork.WithTransactionAsync(async () =>
        {
            await s2SClientRepo.DeleteAsync(request.ClientId, ct);
        }, ct: ct);

        return Unit.Value;
    }
}
