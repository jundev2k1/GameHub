using game_x.domain.Constants;

namespace game_x.persistence.Seeds.Seeders;

public sealed class AppSettingSeeder : ISeeder
{
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        var settings = new[]
        {
            AppSetting.Create(AppSettingConstant.KEY_CLIENT_PAGE_URL, string.Empty, string.Empty, true),
            AppSetting.Create(AppSettingConstant.KEY_TALENT_COMMISSION_RATE, "70", string.Empty, true),
            AppSetting.Create(AppSettingConstant.KEY_UXM_MERCHANT_NUMBER, string.Empty, string.Empty, true),
            AppSetting.Create(AppSettingConstant.KEY_FASTPAY_MERCHANT_NUMBER, string.Empty, string.Empty, true),
        };

        foreach (var setting in settings)
        {
            var isExist = await context.AppSettings
                .AsNoTracking()
                .AnyAsync(sw => sw.Key == setting.Key, ct);
            if (isExist) continue;

            await context.AppSettings.AddAsync(setting, ct);
        }
    }
}