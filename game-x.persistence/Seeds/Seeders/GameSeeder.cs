using game_x.domain.Constants;

namespace game_x.persistence.Seeds.Seeders;

public sealed record GameSeedModel(
    string Name,
    string Code,
    Guid PlatformId
);

public sealed class GameSeeder : ISeeder
{
    private static readonly IReadOnlyList<GameSeedModel> GameData =
    [
        #region 598彩票
        new("韓國快3", "KRF3", GameConstants.PLATFORM_ID_G598),
        new("極速快3", "ESF3", GameConstants.PLATFORM_ID_G598),
        new("澳洲幸運10", "ALPK10", GameConstants.PLATFORM_ID_G598),
        new("極速賽車", "ESPK10", GameConstants.PLATFORM_ID_G598),
        new("幸運飛艇", "LBPK10", GameConstants.PLATFORM_ID_G598),
        new("極速飛艇", "EBPK10", GameConstants.PLATFORM_ID_G598),
        new("動物運動會", "ANPK6", GameConstants.PLATFORM_ID_G598),
        new("三分動物運動會", "M3ANPK6", GameConstants.PLATFORM_ID_G598),
        new("極速時時彩", "ESSSC", GameConstants.PLATFORM_ID_G598),
        new("澳洲幸運5", "ALSSC", GameConstants.PLATFORM_ID_G598),
        new("幸運時時彩", "LBSSC", GameConstants.PLATFORM_ID_G598),
        new("香港六合彩", "HKM6", GameConstants.PLATFORM_ID_G598),
        new("澳門六合彩", "MCM6", GameConstants.PLATFORM_ID_G598),
        new("新澳門六合彩", "NMCM6", GameConstants.PLATFORM_ID_G598),
        new("台灣大樂透", "TWM6", GameConstants.PLATFORM_ID_G598),
        new("香港日日六合彩", "HKDM6", GameConstants.PLATFORM_ID_G598),
        new("福彩3D", "FCD3", GameConstants.PLATFORM_ID_G598),
        new("體彩排列3", "PLD3", GameConstants.PLATFORM_ID_G598),
        new("PC蛋蛋", "ALEGG", GameConstants.PLATFORM_ID_G598),
        new("澳洲幸運10番攤", "AL10FT", GameConstants.PLATFORM_ID_G598),
        new("澳洲幸運5番攤", "AL5FT", GameConstants.PLATFORM_ID_G598),
        #endregion

        #region Baccarat
        new("百家樂", "BAC001", GameConstants.PLATFORM_ID_GAMEBACCARAT),
        #endregion

        #region ETL998
        new("998百家樂", "BAC002", GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT),
        #endregion

        #region SAS Slot
        new("Slot", "SLOT001", GameConstants.PLATFORM_ID_SASSLOT),
        #endregion
  
        #region ATG
        new("台灣麻將", "mahjong", GameConstants.PLATFORM_ID_ATG),
        new("火箭升空", "rocket-crash", GameConstants.PLATFORM_ID_ATG),
        new("泡泡糖", "bubble-gum", GameConstants.PLATFORM_ID_ATG),
        new("戰神賽特", "egyptian-mythology", GameConstants.PLATFORM_ID_ATG),
        new("孫行者", "son-go-ku", GameConstants.PLATFORM_ID_ATG),
        new("赤三國", "scarlet-three-kingdoms", GameConstants.PLATFORM_ID_ATG),
        new("戰神賽特2覺醒之力", "golden-seth", GameConstants.PLATFORM_ID_ATG),
        new("武俠", "wuxia-caishen", GameConstants.PLATFORM_ID_ATG),
        new("骰子比大小", "dice1", GameConstants.PLATFORM_ID_ATG),
        new("通比妞妞", "casino-bull", GameConstants.PLATFORM_ID_ATG),
        new("搶莊妞妞", "banker-bull", GameConstants.PLATFORM_ID_ATG),
        new("賽馬", "horse-racing", GameConstants.PLATFORM_ID_ATG),
        #endregion
    ];
    
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        if (GameData.Count == 0)
            return;

        var existingGames = await context.Games
            .AsNoTracking()
            .Select(g => new
            {
                PlatformId = g.Platform.PublicId,
                g.GameCode
            })
            .ToListAsync(ct);

        var existingGameSet = existingGames
            .Select(x => $"{x.PlatformId}:{x.GameCode}")
            .ToHashSet();

        var platforms = await context.GamePlatforms
            .ToDictionaryAsync(x => x.PublicId, ct);

        var newGames = new List<Game>();

        foreach (var gameSeed in GameData)
        {
            var uniqueKey = $"{gameSeed.PlatformId}:{gameSeed.Code}";

            if (existingGameSet.Contains(uniqueKey))
                continue;

            if (!platforms.TryGetValue(gameSeed.PlatformId, out var platform))
                continue;

            var game = Game.Create(
                gameSeed.Name,
                gameSeed.Code,
                string.Empty,
                string.Empty,
                0);

            game.UpdatePlatform(platform);

            newGames.Add(game);
        }

        if (newGames.Count > 0) await context.Games.AddRangeAsync(newGames, ct);
    }
}