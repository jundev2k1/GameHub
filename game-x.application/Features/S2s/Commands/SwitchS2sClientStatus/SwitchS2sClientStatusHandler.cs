using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.S2s.Commands.SwitchS2sClientStatus;

public sealed class SwitchS2sClientStatusHandler(
    IUnitOfWork unitOfWork,
    IS2sClientRepo s2SClientRepo) : ICommandHandler<SwitchS2sClientStatusCommand>
{
    public async Task<Unit> Handle(SwitchS2sClientStatusCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await s2SClientRepo.UpdateAsync(request.ClientId, s2sClient =>
            {
                s2sClient.UpdateStatus(!s2sClient.IsActive);
            }, ct);
        }, ct: ct);

        return Unit.Value;
    }
}
