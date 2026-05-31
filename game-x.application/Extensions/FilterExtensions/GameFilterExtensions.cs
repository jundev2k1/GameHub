using game_x.share.Extensions;
using System.Linq.Expressions;

namespace game_x.application.Extensions.FilterExtensions;

public static class GameFilterExtensions
{
    public static readonly Dictionary<string, Func<object, Expression<Func<Game, bool>>>> Options =
        new()
        {
            ["types"] = FilterByMultipleTypes,
            ["categories"] = FilterByMultipleCategories,
            ["tags"] = FilterByMultipleTags
        };

    private static Expression<Func<Game, bool>> FilterByMultipleTypes(object value)
    {
        var raw = value.ToStringOrEmpty();
        if (raw.IsNullOrWhiteSpace()) return _ => true;

        var idList = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => Guid.TryParse(s, out _))
            .Select(Guid.Parse)
            .ToList();
        if (idList.Count == 0)
            return _ => false;

        return game => game.GameTypeMappings.Any(gt => idList.Contains(gt.Type.PublicId));
    }

    private static Expression<Func<Game, bool>> FilterByMultipleCategories(object value)
    {
        var raw = value.ToStringOrEmpty();
        if (raw.IsNullOrWhiteSpace()) return _ => true;

        var idList = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => Guid.TryParse(s, out _))
            .Select(Guid.Parse)
            .ToList();
        if (idList.Count == 0)
            return _ => false;

        return game => game.GameCategoryMappings.Any(gc => idList.Contains(gc.Category.PublicId));
    }

    private static Expression<Func<Game, bool>> FilterByMultipleTags(object value)
    {
        var raw = value.ToStringOrEmpty();
        if (raw.IsNullOrWhiteSpace()) return _ => true;

        var idList = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(s => Guid.TryParse(s, out _))
            .Select(Guid.Parse)
            .ToList();
        if (idList.Count == 0)
            return _ => false;

        return game => game.GameTagMappings.Any(gt => idList.Contains(gt.Tag.PublicId));
    }
}