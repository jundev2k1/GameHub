using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AppSettings.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class AppSettingCacheService(
    IMemoryCache cache,
    IAppSettingRepo appSettingRepo) : CacheService(cache), IAppSettingCacheService
{
    private AppSettingDto[] Datasource { get { return Get<AppSettingDto[]>("appSetting:list") ?? []; } }

    public void RefreshCache()
    {
        var data = appSettingRepo.GetAll()
            .Select(s => s.Adapt<AppSettingDto>())
            .ToArray();
        Set("appSetting:list", data);
    }

    public AppSettingDto[] GetAll() => Datasource;

    public AppSettingDto? Get(string key) => Datasource.FirstOrDefault(x => x.Key == key);

    public bool IsExist(string key) => Datasource.Any(x => x.Key == key);

    public string ClientPageUrl =>
        Datasource.FirstOrDefault(i => i.Key == AppSettingConstant.KEY_CLIENT_PAGE_URL)?.Value ?? string.Empty;

    public decimal TalentCommissionRate => decimal.Parse(
        Datasource.FirstOrDefault(i => i.Key == AppSettingConstant.KEY_TALENT_COMMISSION_RATE)?.Value ?? "0");

    public string UxmMerchantNumber =>
        Datasource.FirstOrDefault(i => i.Key == AppSettingConstant.KEY_UXM_MERCHANT_NUMBER)?.Value ?? string.Empty;

    public string FastPayMerchantNumber =>
        Datasource.FirstOrDefault(i => i.Key == AppSettingConstant.KEY_FASTPAY_MERCHANT_NUMBER)?.Value ?? string.Empty;
}
