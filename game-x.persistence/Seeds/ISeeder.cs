namespace game_x.persistence.Seeds;

public interface ISeeder
{
    Task SeedAsync(GameXContext context, CancellationToken ct = default);
}