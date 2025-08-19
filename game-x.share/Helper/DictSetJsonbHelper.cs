using System.Text.Json;

namespace game_x.share.Helper;

/// <summary>
/// Utilities to map <c>Dictionary&lt;string, HashSet&lt;string&gt;&gt;</c>
/// to/from PostgreSQL <c>jsonb</c> and to support EF Core change tracking.
///
/// Intended use:
/// - Plug <see cref="ToJson"/> / <see cref="FromJson"/> into a ValueConverter
/// - Plug <see cref="Equals(Dictionary{string, HashSet{string}}?, Dictionary{string, HashSet{string}}?)"/>,
///   <see cref="GetHashCode(Dictionary{string, HashSet{string}}?)"/>, <see cref="Snapshot"/> into a ValueComparer
///
/// Design goals:
/// - Keep domain semantics as sets (uniqueness, O(1) add/remove/contains)
/// - Structural equality (order-insensitive) without JSON serialization
/// - Stable hash code independent of ordering
/// </summary>
public static class DictSetJsonbHelper
{
    // Serializer options for compact, consistent JSON.
    private static readonly JsonSerializerOptions Opts = new(JsonSerializerDefaults.Web);

    // Case-sensitive, culture-invariant comparison is the safest for IDs/emojis.
    private static readonly StringComparer Cmp = StringComparer.Ordinal;

    // ---------------------------------------------------------------------
    // Converter: CLR <-> jsonb
    // ---------------------------------------------------------------------

    /// <summary>Serialize the dictionary-of-sets to a JSON string for jsonb column.</summary>
    public static string ToJson(Dictionary<string, HashSet<string>>? value)
        => JsonSerializer.Serialize(value ?? new Dictionary<string, HashSet<string>>(Cmp), Opts);

    /// <summary>Deserialize JSON string from jsonb column back into dictionary-of-sets.</summary>
    public static Dictionary<string, HashSet<string>> FromJson(string json)
    {
        // Deserialize to arrays first (JSON does not have a set type).
        var raw = JsonSerializer.Deserialize<Dictionary<string, string[]>>(json, Opts)
                  ?? new Dictionary<string, string[]>();

        var dict = new Dictionary<string, HashSet<string>>(Cmp);
        foreach (var kv in raw)
        {
            var arr = kv.Value;
            dict[kv.Key] = new HashSet<string>(arr, Cmp);
        }
        return dict;
    }

    // ---------------------------------------------------------------------
    // Comparer: structural equality & stable hash code (no serialization)
    // ---------------------------------------------------------------------

    /// <summary>
    /// Structural equality: same keys and, for each key, the same set of values
    /// (order-insensitive). Treats null sets as empty.
    /// </summary>
    public static bool Equals(
        Dictionary<string, HashSet<string>>? a,
        Dictionary<string, HashSet<string>>? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;
        if (a.Count != b.Count) return false;

        foreach (var kv in a)
        {
            if (!b.TryGetValue(kv.Key, out var setB)) return false;

            var sa = kv.Value;
            var sb = setB;

            if (!sa.SetEquals(sb)) return false; // order-insensitive
        }
        return true;
    }

    /// <summary>
    /// Stable, order-insensitive hash code aligned with <see cref="Equals"/>.
    /// Sorts keys and each set before mixing to keep determinism.
    /// </summary>
    public static int GetHashCode(Dictionary<string, HashSet<string>>? value)
    {
        if (value is null || value.Count == 0) return 0;

        unchecked
        {
            var hash = 17;

            // Sort keys to make hash independent of dictionary enumeration order
            foreach (var key in value.Keys.OrderBy(k => k, Cmp))
            {
                hash = hash * 31 + Cmp.GetHashCode(key);

                // Get set once and hash its sorted contents
                value.TryGetValue(key, out var set);
                if (set is { Count: > 0 })
                {
                    foreach (var s in set.OrderBy(x => x, Cmp))
                        hash = hash * 31 + Cmp.GetHashCode(s);
                }
            }

            return hash;
        }
    }

    /// <summary>
    /// Deep clone the dictionary-of-sets. EF needs a snapshot to detect later
    /// in-place mutations (add/remove within sets).
    /// </summary>
    public static Dictionary<string, HashSet<string>> Snapshot(
        Dictionary<string, HashSet<string>> value)
    {
        var clone = new Dictionary<string, HashSet<string>>(value.Count, Cmp);
        foreach (var kv in value)
        {
            clone[kv.Key] = new HashSet<string>(kv.Value, Cmp);
        }
        return clone;
    }
}
