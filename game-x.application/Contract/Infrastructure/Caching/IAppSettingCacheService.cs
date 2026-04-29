using game_x.application.Features.AppSettings.DTOs;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IAppSettingCacheService
{
    void RefreshCache();

    AppSettingDto[] GetAll();

    AppSettingDto? Get(string key);

    bool IsExist(string key);

    string ClientPageUrl { get; }
    decimal TalentCommissionRate { get; }

    string UxmMerchantNumber { get; }
    string FastPayMerchantNumber { get; }
}
