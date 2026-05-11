namespace game_x.persistence.Seeds.Seeders;

public sealed class GameTypeSeeder : ISeeder
{
    private static readonly Dictionary<string, Guid> GameTypes = new()
    {
        { "快三", Guid.Parse("4bafec6b-68d3-4f20-832d-3e9f4a1d1e25") },
        { "PK10", Guid.Parse("2f7e94a6-69ab-4f64-8b30-9fd563c60b41") },
        { "PK6", Guid.Parse("67d3a8f4-8fb2-4e84-9b52-1c9f82c1f46a") },
        { "時時彩", Guid.Parse("a7f91d64-058b-4b7e-88f4-d78a0b493d12") },
        { "六合彩", Guid.Parse("e3f9d6c7-1b5a-4629-bc8e-2f6b1d4c93e0") },
        { "3D", Guid.Parse("51a8b2d6-2982-4a7e-9c2f-7f38e1a0d1f5") },
        { "蛋蛋", Guid.Parse("8e9b1f62-4d38-4f7a-bc19-2e3a1b4f8c7d") },
    };
    
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        if (await context.GameTypes.AnyAsync(ct)) return;

        var gameTypes = new List<GameType>
        {
            GameType.Create("快三", string.Empty, string.Empty, 0, GameTypes["快三"]),
            GameType.Create("PK10", string.Empty, string.Empty, 0, GameTypes["PK10"]),
            GameType.Create("PK6", string.Empty, string.Empty, 0, GameTypes["PK6"]),
            GameType.Create("時時彩", string.Empty, string.Empty, 0, GameTypes["時時彩"]),
            GameType.Create("六合彩", string.Empty, string.Empty, 0, GameTypes["六合彩"]),
            GameType.Create("3D", string.Empty, string.Empty, 0, GameTypes["3D"]),
            GameType.Create("蛋蛋", string.Empty, string.Empty, 0, GameTypes["蛋蛋"]),
        };
        await context.GameTypes.AddRangeAsync(gameTypes, ct);
    }
}