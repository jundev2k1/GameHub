namespace game_x.share.Extensions;

public static class StringExtension
{
    public static bool IsNullOrEmpty(this string? input)
        => string.IsNullOrEmpty(input);

    public static bool IsNotNullOrEmpty(this string? input)
        => !string.IsNullOrEmpty(input);

    public static bool IsNullOrWhiteSpace(this string? input)
        => string.IsNullOrWhiteSpace(input);

    public static string ToStringOrEmpty(this object? input)
        => input?.ToString() ?? string.Empty;

    public static string JoinToString(this IEnumerable<string> input, string separater = ",")
        => string.Join(separater, input);
    public static string JoinToString<T>(this IEnumerable<T> input, Func<T, string> callback, string separater)
        => string.Join(separater, input.Select(x => callback(x)));
}
