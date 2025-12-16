using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AppSettings.DTOs;

namespace game_x.application.Features.AppSettings.Queries.GetAllSettings;

public sealed class GetAllSettingsHandler(IAppSettingRepo appSettingRepo) : IQueryHandler<GetAllSettingsQuery, AppSettingDto[]>
{
    public async Task<AppSettingDto[]> Handle(GetAllSettingsQuery request, CancellationToken ct = default)
    {
        var data = await appSettingRepo.GetAllAsync(ct);
        return [.. data.Select(i => i.Adapt<AppSettingDto>())];
    }
}
