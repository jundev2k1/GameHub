namespace game_x.application.Common.Filters;

public static class QueryConverter
{
    public static IEnumerable<QueryFilter> ToFilters(
        IEnumerable<string>? filters,
        string? searchText = null,
        Dictionary<string, string>? @params = null,
        string[]? ignoreFields = null)
    {
        // Create filters from the provided string collection
        if ((filters != null) && filters.Any())
        {
            foreach (var filter in filters)
            {
                var parts = filter.Split("~");
                if (parts.Length != 3) continue;

                yield return new QueryFilter(
                    Field: parts[0],
                    Operator: parts[1],
                    Value: parts[2]);
            }
        }

        // Add search by keyword if searchText is provided
        if (searchText is not null)
        {
            yield return new QueryFilter(
                Field: "search",
                Operator: string.Empty,
                Value: searchText);
        }

        if (@params != null)
        {
            foreach (var param in @params)
            {
                if ((ignoreFields != null)
                    && ignoreFields.Any(@if => @if.Equals(param.Key, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                yield return new QueryFilter(
                    Field: param.Key,
                    Operator: string.Empty,
                    Value: param.Value);
            }
        }
    }

    public static IEnumerable<QuerySort> ToSorts(IEnumerable<string>? sorts)
    {
        if (sorts is null || !sorts.Any()) yield break;

        // Create sorts from the provided string collection
        foreach (var sort in sorts)
        {
            var parts = sort.Split(":");
            if (parts.Length != 2) continue;

            yield return new QuerySort(
                Field: parts[0],
                Direction: parts[1]);
        }
    }
}