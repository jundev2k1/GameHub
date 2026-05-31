using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.S2s.Commands.SwitchS2sClientSettingStatus;

public sealed class SwitchS2sClientSettingStatusHandler(
    IUnitOfWork unitOfWork,
    IS2sClientSettingRepo s2SClientSettingRepo) : ICommandHandler<SwitchS2sClientSettingStatusCommand>
{
    public async Task<Unit> Handle(SwitchS2sClientSettingStatusCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await s2SClientSettingRepo.UpdateAsync(request.AppCode, setting =>
            {
                if (setting.ClientId != request.ClientId)
                    throw new NotFoundException("Client ID was not found.");

                setting.UpdateStatus(!setting.IsActive);
            }, ct);
        }, ct: ct);

        return Unit.Value;
    }
}
