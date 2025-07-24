namespace game_x.application.Common;

public static class CurrencyCodeProvider
{
    public record CurrencyCodeDto(string Code, string Name, string CountryCode, string EmojiFlag);

    private static readonly List<CurrencyCodeDto> _currencies =
    [
        new(CurrencyCode.TWD, "New Taiwan Dollar", "TW", "🇹🇼"),
        new(CurrencyCode.HKD, "Hong Kong Dollar", "HK", "🇭🇰"),
        new(CurrencyCode.JPY, "Japanese Yen", "JP", "🇯🇵"),
        new(CurrencyCode.CNY, "Chinese Yuan", "CN", "🇨🇳"),
        new(CurrencyCode.SGD, "Singapore Dollar", "SG", "🇸🇬"),
        new(CurrencyCode.MYR, "Malaysian Ringgit", "MY", "🇲🇾"),
        new(CurrencyCode.KRW, "South Korean Won", "KR", "🇰🇷"),
        new(CurrencyCode.THB, "Thai Baht", "TH", "🇹🇭"),
        new(CurrencyCode.VND, "Vietnamese Dong", "VN", "🇻🇳"),
        new(CurrencyCode.USD, "US Dollar", "US", "🇺🇸"),
        new(CurrencyCode.GBP, "British Pound", "GB", "🇬🇧"),
        new(CurrencyCode.PHP, "Philippine Peso", "PH", "🇵🇭")
    ];

    public static List<CurrencyCodeDto> All() => _currencies;
}