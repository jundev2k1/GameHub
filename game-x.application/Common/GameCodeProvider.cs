namespace game_x.application.Common;

public static class GameCodeProvider
{
    public record GameCodeDto(string Category, string GameType, string GamePlatform, string GameName, string GameCode, int Priority);

    private static readonly List<GameCodeDto> _gameCodes =
    [
        new("彩票", "快三",   "598彩票", "韓國快3",       GameCode.KRF3,    1),
        new("彩票", "快三",   "598彩票", "極速快3",       GameCode.ESF3,    2),
        new("彩票", "PK10",  "598彩票", "澳洲幸運10",     GameCode.ALPK10,  1),
        new("彩票", "PK10",  "598彩票", "極速賽車",       GameCode.ESPK10,  1),
        new("彩票", "PK10",  "598彩票", "幸運飛艇",       GameCode.LBPK10,  1),
        new("彩票", "PK10",  "598彩票", "極速飛艇",       GameCode.EBPK10,  1),
        new("彩票", "PK6",   "598彩票", "動物運動會",     GameCode.ANPK6,   1),
        new("彩票", "PK6",   "598彩票", "三分動物運動會", GameCode.M3ANPK6, 1),
        new("彩票", "時時彩","598彩票", "極速時時彩",     GameCode.ESSSC,   1),
        new("彩票", "時時彩","598彩票", "澳洲幸運5",      GameCode.ALSSC,   1),
        new("彩票", "時時彩","598彩票", "幸運時時彩",     GameCode.LBSSC,   1),
        new("彩票", "六合彩", "598彩票", "香港六合彩",     GameCode.HKM6,    1),
        new("彩票", "六合彩", "598彩票", "澳門六合彩",     GameCode.MCM6,    1),
        new("彩票", "六合彩", "598彩票", "新澳門六合彩",   GameCode.NMCM6,   1),
        new("彩票", "六合彩", "598彩票", "台灣大樂透",     GameCode.TWM6,    1),
        new("彩票", "六合彩", "598彩票", "香港日日六合彩", GameCode.HKDM6,   1),
        new("彩票", "3D",    "598彩票", "福彩3D",        GameCode.FCD3,    1),
        new("彩票", "3D",    "598彩票", "體彩排列3",     GameCode.PLD3,    1),
        new("彩票", "蛋蛋",  "598彩票", "PC蛋蛋",        GameCode.ALEGG,   1),
        new("彩票", "番攤",  "598彩票", "澳洲幸運10番攤", GameCode.AL10FT,  5),
        new("彩票", "番攤",  "598彩票", "澳洲幸運5番攤",  GameCode.AL5FT,   5)
    ];

    public static List<GameCodeDto> All() => _gameCodes
        .OrderBy(g => g.Priority)
        .ToList();
}
