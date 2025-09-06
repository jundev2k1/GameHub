using game_x.application.Features.Games.Dtos;
using game_x.share.Extensions;
using System.Linq.Expressions;

namespace game_x.application.Extensions.FilterExtensions;

public static class GameFilterExtensions
{
    public static readonly Dictionary<string, Func<object, Expression<Func<GameInfoDto, bool>>>> Options =
        new()
        {
            ["types"] = FilterByMultipleTypes,
            ["categories"] = FilterByMultipleCategories,
            ["tags"] = FilterByMultipleTags
        };

    private static Expression<Func<GameInfoDto, bool>> FilterByMultipleTypes(object value)
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

        return game => game.GameTypes.Any(gt => idList.Contains(gt.Id));
    }

    private static Expression<Func<GameInfoDto, bool>> FilterByMultipleCategories(object value)
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

        return game => game.Categories.Any(gt => idList.Contains(gt.Id));
    }

    private static Expression<Func<GameInfoDto, bool>> FilterByMultipleTags(object value)
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

        return game => game.GameTags.Any(gt => idList.Contains(gt.Id));
    }
}