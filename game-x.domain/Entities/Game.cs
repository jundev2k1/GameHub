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
    public int? ThumbnailId { get; private set; }
    public MediaFile? Thumbnail { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<GameCategoryMapping> GameCategoryMappings { get; private set; } = default!;
    public ICollection<GameTypeMapping> GameTypeMappings { get; private set; } = default!;
    public ICollection<GameTagMapping> GameTagMappings { get; private set; } = default!;

    public static Game Create(string name, string gameCode, string desc, string note, int priority, int? thumbnailId = null)
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
            ThumbnailId = thumbnailId,
        };
    }

    public void UpdateGame(
        string name,
        string desc,
        string note,
        int priority,
        bool isActive,
        ICollection<GameCategoryMapping>? categories,
        ICollection<GameTypeMapping>? types,
        ICollection<GameTagMapping>? tags,
        int? thumbnailId = null)
    {
        if (priority < 0)
            throw new ArgumentException("Priority must be greater than or equal to 0.", nameof(priority));
        if (categories != null && (categories.Count == 0 || categories.Count(c => c.IsPrimary) != 1))
            throw new ArgumentException("There must be exactly one primary category.", nameof(categories));
        if (types != null &&  (types.Count == 0 || types.Count(t => t.IsPrimary) != 1))
            throw new ArgumentException("There must be exactly one primary type.", nameof(types));
        if (tags != null && (tags.Count == 0 || tags.Count(t => t.IsPrimary) != 1))
            throw new ArgumentException("There must be exactly one primary tag.", nameof(tags));
        if (categories != null && (categories.Select(c => c.CategoryId).Distinct().Count() != categories.Count))
            throw new ArgumentException("Categories contains duplicate category IDs.", nameof(categories));
        if (types != null && (types.Select(t => t.TypeId).Distinct().Count() != types.Count))
            throw new ArgumentException("Types contains duplicate type IDs.", nameof(types));
        if (tags != null && (tags.Select(t => t.TagId).Distinct().Count() != tags.Count))
            throw new ArgumentException("Tags contains duplicate tag IDs.", nameof(tags));

        Name = name;
        Description = desc;
        Note = note;
        Priority = priority;
        IsActive = isActive;
        ThumbnailId = thumbnailId ?? ThumbnailId;
        GameCategoryMappings = categories ?? GameCategoryMappings;
        GameTypeMappings = types ?? GameTypeMappings;
        GameTagMappings = tags ?? GameTagMappings;
    }

    public void UpdateThumbnail(MediaFile thumbnail)
    {
        Thumbnail = thumbnail;
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

        var gameCategoryMapping = GameCategoryMapping.Create(Id, category.Id);
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

        var gameTypeMapping = GameTypeMapping.Create(Id, type.Id);
        var hasPrimary = GameTypeMappings.Any(c => c.IsPrimary);
        if (!hasPrimary)
            gameTypeMapping.SetPrimary(true);

        GameTypeMappings.Add(gameTypeMapping);
    }

    public void AddTag(GameTag tag, bool isPrimary, int priority)
    {
        ArgumentNullException.ThrowIfNull(tag, nameof(tag));

        GameTagMappings ??= [];
        if (GameTagMappings.Any(t => t.Tag.PublicId == tag.PublicId))
            throw new ArgumentException($"Tag {tag.Name} already exists in the game.");

        GameTagMappings.Add(GameTagMapping.Create(Id, tag.Id, isPrimary, priority));
    }

    public void SetPrimaryTag(Guid tagId)
    {
        if (GameTagMappings.Any(t => t.Tag.PublicId == tagId))
            throw new ArgumentException($"Tag with PublicId {tagId} does not exist in the game.");

        foreach (var m in GameTagMappings!)
        {
            m.SetPrimary(m.Tag.PublicId == tagId);
        }
    }

    public void RemoveTag(GameTag tag)
    {
        var mapping = GameTagMappings?.FirstOrDefault(t => t.Tag.PublicId == tag.PublicId);
        if (mapping is not null)
        {
            GameTagMappings!.Remove(mapping);
        }
    }
}
