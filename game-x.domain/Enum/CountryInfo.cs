namespace game_x.domain.Enum;

public class CountryInfo
{
    public static readonly List<CountryInfo> AllCountries = new()
    {
        new CountryInfo("Afghanistan", "+93", "AF", 93),
        new CountryInfo("Åland Islands", "+358", "AX", 358),
        new CountryInfo("Albania", "+355", "AL", 355),
        new CountryInfo("Algeria", "+213", "DZ", 213),
        new CountryInfo("American Samoa", "+1684", "AS", 1684),
        new CountryInfo("Andorra", "+376", "AD", 376),
        new CountryInfo("Angola", "+244", "AO", 244),
        new CountryInfo("Anguilla", "+1264", "AI", 1264),
        new CountryInfo("Antarctica", "+672", "AQ", 672),
        new CountryInfo("Antigua and Barbuda", "+1268", "AG", 1268),
        new CountryInfo("Taiwan", "+886", "TW", 886),
        new CountryInfo("Hong Kong", "+852", "HK", 852),
        new CountryInfo("Macao", "+853", "MO", 853),
        new CountryInfo("China", "+86", "CN", 86),
        new CountryInfo("Japan", "+81", "JP", 81),
        new CountryInfo("South Korea", "+82", "KR", 82),
        new CountryInfo("United States", "+1", "US", 1)
    };

    private CountryInfo(string countryName, string phoneCode, string countryCode, int phoneCodeInt)
    {
        CountryName = countryName;
        PhoneCode = phoneCode;
        CountryCode = countryCode;
        PhoneCodeInt = phoneCodeInt;
    }

    public string CountryName { get; }
    public string PhoneCode { get; }
    public string CountryCode { get; }
    public int PhoneCodeInt { get; }
}