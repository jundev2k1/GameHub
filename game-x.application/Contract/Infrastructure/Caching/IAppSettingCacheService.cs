using game_x.application.Features.AppSettings.DTOs;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IAppSettingCacheService
{
    void RefreshCacheAsync(CancellationToken ct = default);

    AppSettingDto[] GetAllAsync();

    AppSettingDto? GetAsync(string key);

    bool IsExistSetting(string key);

    decimal TalentCommissionRate { get; }
}
