namespace game_x.domain.Entities;

public sealed class NavigationItemTranslation : BaseEntity<int>, IAuditable
{
    public int NavigationItemId { get; private set; }
    public NavigationItem NavigationItem { get; private set; } = default!;
    public LanguageCode LanguageCode { get; private set; } = default!;
    public string Title { get; private set; } = string.Empty;

    public static NavigationItemTranslation Create(
        int navigationItemId,
        LanguageCode languageCode,
        string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));

        return new NavigationItemTranslation
        {
            NavigationItemId = navigationItemId,
            LanguageCode = languageCode,
            Title = title
        };
    }

    public void Update(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));

        Title = title;
    }
}
