using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.S2s.Commands.UpdateS2sClient;

public sealed class UpdateS2sClientHandler(
    IUnitOfWork unitOfWork,
    IS2sClientRepo s2SClientRepo) : ICommandHandler<UpdateS2sClientCommand>
{
    public async Task<Unit> Handle(UpdateS2sClientCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await s2SClientRepo.UpdateAsync(request.ClientId!, s2sClient =>
            {
                s2sClient.UpdateInfo(request.ClientName, request.ClientCode, request.Notes);
            }, ct);
        }, ct: ct);

        return Unit.Value;
    }
}
