using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace Test.Common.Settings;

public static class MockSettings
{
    public static readonly IOptions<GameXSettings> GameX =
        Options.Create(new GameXSettings { MerchantNumber = "M-12345" });
}