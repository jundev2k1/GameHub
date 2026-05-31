namespace game_x.application.Features.Games.Dtos;

public record GameCategoryItemDto(Guid Id, string Name, bool IsPrimary);

public record GameTypeItemDto(Guid Id, string Name, bool IsPrimary);

public record GameTagItemDto(Guid Id, string Name, string Icon, string Color, bool IsPrimary);

public record GameMediaItemDto(GameMediaType Type, GameMediaCategory Category, string FileName, string? Url, string Metadata, string Title);

public record GameItemDto(
    Guid Id,
    string GameCode,
    string Name,
    string Description,
    Guid PlatformId,
    string PlatformName,
    string? GameThumbnailUrl,
    GameCategoryItemDto[] Categories,
    GameTypeItemDto[] GameTypes,
    GameTagItemDto[] Tags,
    GameMediaItemDto[] MediaItems)
{
    public GameItemDto(GameInfoDto game)
        : this(
            game.Id,
            game.GameCode,
            game.Name,
            game.Description,
            game.PlatformId,
            game.PlatformName,
            game.Thumbnail?.Url,
            [],
            [],
            [],
            [])
    {
        Categories = [.. game.Categories
            .OrderByDescending(cate => cate.IsPrimary)
            .ThenByDescending(cate => cate.Priority)
            .Select(cate => new GameCategoryItemDto(cate.Id, cate.Name, cate.IsPrimary))];

        GameTypes = [.. game.GameTypes
            .OrderByDescending(type => type.IsPrimary)
            .ThenByDescending(type => type.Priority)
            .Select(type => new GameTypeItemDto(type.Id, type.Name, type.IsPrimary))];

        Tags = [.. game.GameTags
            .OrderByDescending(tag => tag.IsPrimary)
            .ThenByDescending(tag => tag.Priority)
            .Select(tag => new GameTagItemDto(tag.Id, tag.Name, tag.Icon, tag.Color, tag.IsPrimary))];

        MediaItems = [.. game.GameMediaItems
            .OrderByDescending(m => m.Priority)
            .Select(m => new GameMediaItemDto(m.Type, m.Category, m.FileName, m.Url, m.Metadata, m.Title))];
    }
}
