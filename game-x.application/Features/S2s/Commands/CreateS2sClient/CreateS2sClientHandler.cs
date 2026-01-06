using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.S2s.Commands.CreateS2sClient;

public sealed class CreateS2sClientHandler(
    IUnitOfWork unitOfWork,
    IS2sClientRepo s2SClientRepo) : ICommandHandler<CreateS2sClientCommand>
{
    public async Task<Unit> Handle(CreateS2sClientCommand request, CancellationToken ct = default)
    {
        var entity = S2SClient.Create(
            request.ClientName.Trim(),
            request.ClientCode.Trim(),
            request.Notes.Trim());
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await s2SClientRepo.CreateAsync(entity, ct);
        }, ct: ct);

        return Unit.Value;
    }
}
