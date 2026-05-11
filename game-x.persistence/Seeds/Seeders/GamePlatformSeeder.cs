using game_x.domain.Constants;

namespace game_x.persistence.Seeds.Seeders;

public sealed class GamePlatformSeeder : ISeeder
{
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        var gamePlatforms = new[]
        {
            GamePlatform.Create("598彩票", string.Empty, string.Empty, 0, GameConstants.PLATFORM_ID_G598),
            GamePlatform.Create("百家樂", string.Empty, string.Empty, 0, GameConstants.PLATFORM_ID_GAMEBACCARAT),
            GamePlatform.Create("etl998", string.Empty, string.Empty, 0, GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT),
            GamePlatform.Create("SAS Slot", string.Empty, string.Empty, 0, GameConstants.PLATFORM_ID_SASSLOT),
            GamePlatform.Create("ATG", string.Empty, string.Empty, 0, GameConstants.PLATFORM_ID_ATG),
        };
        foreach (var gamePlatform in gamePlatforms)
        {
            var isExist = await context.GamePlatforms
                .AnyAsync(p => p.PublicId == gamePlatform.PublicId, ct);
            if (isExist) continue;

            await context.GamePlatforms.AddAsync(gamePlatform, ct);
        }
    }
}