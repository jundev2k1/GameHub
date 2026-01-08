using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.S2s.Commands.UpdateS2sClientSetting;

public sealed class UpdateS2sClientSettingHandler(
    IUnitOfWork unitOfWork,
    IS2sClientSettingRepo s2SClientSettingRepo) : ICommandHandler<UpdateS2sClientSettingCommand>
{
    public async Task<Unit> Handle(UpdateS2sClientSettingCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await s2SClientSettingRepo.UpdateAsync(request.AppCode!, setting =>
            {
                if (setting.ClientId != request.ClientId)
                    throw new NotFoundException("Client ID was not found.");

                setting.UpdateInfo(request.AppName.Trim(), request.Notes.Trim());
                setting.UpdateConfig(request.Host, AllowedIp.Of(request.AllowedIps));
            }, ct);
        }, ct: ct);

        return Unit.Value;
    }
}
