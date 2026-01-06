using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.S2s.Commands.CreateS2sClientSetting;

public sealed class CreateS2sClientSettingHandler(
    IUnitOfWork unitOfWork,
    IS2sClientRepo s2SClientRepo,
    IS2sClientSettingRepo s2SClientSettingRepo) : ICommandHandler<CreateS2sClientSettingCommand>
{
    public async Task<Unit> Handle(CreateS2sClientSettingCommand request, CancellationToken ct = default)
    {
        var isExistParent = await s2SClientRepo.IsExistAsync(request.ClientId!, ct);
        if (!isExistParent) throw new NotFoundException("Client ID was not found.");

        var entity = S2SClientSetting.Create(
            request.ClientId!,
            request.AppCode.Trim(),
            request.AppName.Trim(),
            request.Host.Trim(),
            AllowedIp.Of(request.AllowIps),
            request.Notes.Trim());
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await s2SClientSettingRepo.CreateAsync(entity, ct);
        }, ct: ct);

        return Unit.Value;
    }
}
