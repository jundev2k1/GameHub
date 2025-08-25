namespace game_x.domain.Entities;

public sealed class Game : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string GameCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int PlatformId { get; private set; }
    public GamePlatform Platform { get; private set; } = default!;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;
    public ICollection<GameCategoryMapping> GameCategoryMappings { get; private set; } = default!;
    public ICollection<GameTypeMapping> GameTypeMappings { get; private set; } = default!;

    public static Game Create(string name, string gameCode, string desc, string note, int priority)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(gameCode, nameof(gameCode));

        return new Game
        {
            GameCode = gameCode,
            Name = name,
            Description = desc,
            Note = note,
            Priority = priority,
        };
    }

    public void UpdatePlatform(GamePlatform platform)
    {
        ArgumentNullException.ThrowIfNull(platform, nameof(platform));

        Platform = platform;
        PlatformId = platform.Id;
    }

    public void AddCategory(GameCategory category)
    {
        ArgumentNullException.ThrowIfNull(category, nameof(category));

        GameCategoryMappings ??= [];
        if (GameCategoryMappings.Any(c => c.Category.PublicId == category.PublicId))
            throw new InvalidOperationException($"Category with PublicId {category.PublicId} already exists in the game.");

        var gameCategoryMapping = GameCategoryMapping.Create(this, category);
        var hasPrimary = GameCategoryMappings.Any(c => c.IsPrimary);
        if (!hasPrimary)
            gameCategoryMapping.SetPrimary(true);

        GameCategoryMappings.Add(gameCategoryMapping);
    }

    public void AddType(GameType type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        GameTypeMappings ??= [];
        if (GameTypeMappings.Any(t => t.Type.PublicId == type.PublicId))
            throw new InvalidOperationException($"Type with PublicId {type.PublicId} already exists in the game.");

        var gameTypeMapping = GameTypeMapping.Create(this, type);
        var hasPrimary = GameTypeMappings.Any(c => c.IsPrimary);
        if (!hasPrimary)
            gameTypeMapping.SetPrimary(true);

        GameTypeMappings.Add(gameTypeMapping);
    }
}
