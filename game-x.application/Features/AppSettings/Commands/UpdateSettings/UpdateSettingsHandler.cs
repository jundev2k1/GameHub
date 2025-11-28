using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.AppSettings.Commands.UpdateSettings;

public sealed class UpdateSettingsHandler(
    IUnitOfWork unitOfWork,
    IAppSettingRepo appSettingRepo,
    IAppSettingCacheService appSettingCache) : ICommandHandler<UpdateSettingsCommand>
{
    public async Task<Unit> Handle(UpdateSettingsCommand request, CancellationToken ct = default)
    {
        var isExist = request.Settings.All(i => appSettingCache.IsExist(i.Key));
        if (!isExist) throw new BadRequestException("One or more settings do not exist in the DB.");

        await unitOfWork.WithTransactionAsync(async () =>
        {
            foreach (var setting in request.Settings)
            {
                await appSettingRepo.UpdateAsync(setting.Key, appSetting =>
                {
                    if (appSetting.Value != setting.Value && !appSetting.IsEditable)
                        throw new BadRequestException("This setting not allow to edit.");

                    appSetting.Update(setting.Value, setting.Description);
                }, ct);
            }
        }, ct);

        appSettingCache.RefreshCache();

        return Unit.Value;
    }
}
