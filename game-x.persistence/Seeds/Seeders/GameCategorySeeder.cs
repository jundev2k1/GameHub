namespace game_x.persistence.Seeds.Seeders;

public sealed class GameCategorySeeder : ISeeder
{
    private static readonly Dictionary<string, Guid> GameCategories = new()
    {
        { "彩票", Guid.Parse("9f1f6c6e-25b5-4d40-9a07-8de1e56afec4") },
    };
    
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        if (await context.GameCategories.AnyAsync(ct)) return;

        var gameCategories = new []
        {
            GameCategory.Create("彩票", "Lottery", string.Empty, 0, GameCategories["彩票"])
        };
        await context.GameCategories.AddRangeAsync(gameCategories);
    }
}