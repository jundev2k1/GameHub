namespace game_x.application.Common;

public static class GameCodeProvider
{
    public record GameCodeDto(string GameCode, string Name, string Category, string Icon, string Provider, int Priority);

    private static readonly List<GameCodeDto> _gameCodes =
    [
        new(GameCode.KRF3,   "韓國快3",       "Fast3",  "🎲",  "G598", 1),
        new(GameCode.ESF3,   "極速快3",       "Fast3",  "⚡",  "G598", 2),
        new(GameCode.ALPK10, "澳洲幸運10",     "PK10",   "🍀",  "G598", 1),
        new(GameCode.ESPK10, "極速賽車",       "PK10",   "🏎️", "G598", 2),
        new(GameCode.LBPK10, "幸運飛艇",       "PK10",   "🚁", "G598", 3),
        new(GameCode.EBPK10, "極速飛艇",       "PK10",   "✈️", "G598", 4),
        new(GameCode.ANPK6,  "動物運動會",     "Animal", "🐾", "G598", 1),
        new(GameCode.M3ANPK6,"三分動物運動會", "Animal", "🦁", "G598", 2),
        new(GameCode.ESSSC,  "極速時時彩",     "SSC",    "⏰", "G598", 1),
        new(GameCode.ALSSC,  "澳洲幸運5",     "SSC",    "🎯", "G598", 2),
        new(GameCode.LBSSC,  "幸運時時彩",     "SSC",    "🎰", "G598", 3),
        new(GameCode.HKM6,   "香港六合彩",     "MarkSix","🇭🇰", "G598", 1),
        new(GameCode.MCM6,   "澳門六合彩",     "MarkSix","🇲🇴", "G598", 2),
        new(GameCode.NMCM6,  "新澳門六合彩",   "MarkSix","🆕", "G598", 3),
        new(GameCode.TWM6,   "台灣大樂透",     "MarkSix","🇹🇼", "G598", 4),
        new(GameCode.HKDM6,  "香港日日六合彩", "MarkSix","📅", "G598", 5),
        new(GameCode.FCD3,   "福彩3D",        "Lottery","🎫", "G598", 1),
        new(GameCode.PLD3,   "體彩排列3",     "Lottery","🏆", "G598", 2),
        new(GameCode.ALEGG,  "PC蛋蛋",        "Special","🥚", "G598", 1),
        new(GameCode.AL10FT, "澳洲幸運10番攤","FanTan","🎴", "G598", 1),
        new(GameCode.AL5FT,  "澳洲幸運5番攤", "FanTan","🃏", "G598", 2)
    ];

    public static List<GameCodeDto> All() => _gameCodes
        .OrderBy(g => g.Priority)
        .ToList();
}
