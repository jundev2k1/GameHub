namespace game_x.application.Common;

public static class GameCodeProvider
{
    public record GameCodeDto(string GameCode, string Name, string Category, string Icon);

    private static readonly List<GameCodeDto> _gameCodes =
    [
        new(GameCode.KRF3, "韓國快3", "Fast3", "🎲"),
        new(GameCode.ESF3, "極速快3", "Fast3", "⚡"),
        new(GameCode.ALPK10, "澳洲幸運10", "PK10", "🍀"),
        new(GameCode.ESPK10, "極速賽車", "PK10", "🏎️"),
        new(GameCode.LBPK10, "幸運飛艇", "PK10", "🚁"),
        new(GameCode.EBPK10, "極速飛艇", "PK10", "✈️"),
        new(GameCode.ANPK6, "動物運動會", "Animal", "🐾"),
        new(GameCode.M3ANPK6, "三分動物運動會", "Animal", "🦁"),
        new(GameCode.ESSSC, "極速時時彩", "SSC", "⏰"),
        new(GameCode.ALSSC, "澳洲幸運5", "SSC", "🎯"),
        new(GameCode.LBSSC, "幸運時時彩", "SSC", "🎰"),
        new(GameCode.HKM6, "香港六合彩", "MarkSix", "🇭🇰"),
        new(GameCode.MCM6, "澳門六合彩", "MarkSix", "🇲🇴"),
        new(GameCode.NMCM6, "新澳門六合彩", "MarkSix", "🆕"),
        new(GameCode.TWM6, "台灣大樂透", "MarkSix", "🇹🇼"),
        new(GameCode.HKDM6, "香港日日六合彩", "MarkSix", "📅"),
        new(GameCode.FCD3, "福彩3D", "Lottery", "🎫"),
        new(GameCode.PLD3, "體彩排列3", "Lottery", "🏆"),
        new(GameCode.ALEGG, "PC蛋蛋", "Special", "🥚"),
        new(GameCode.AL10FT, "澳洲幸運10番攤", "FanTan", "🎴"),
        new(GameCode.AL5FT, "澳洲幸運5番攤", "FanTan", "🃏")
    ];

    public static List<GameCodeDto> All() => _gameCodes;
}