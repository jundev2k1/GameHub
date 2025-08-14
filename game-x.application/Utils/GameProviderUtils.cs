
namespace game_x.application.Utils;

internal sealed class GameProviderUtils
{
    public static string SnoGenerate()
    {
        // Format: GX_ (3) + yyyyMMddHHmmss (14) + 10 random digits = 27 chars total
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var randomPart = Random.Shared.NextInt64(1000000000L, 9999999999L).ToString("D10");

        return $"GX_{timestamp}{randomPart}";
    }
}