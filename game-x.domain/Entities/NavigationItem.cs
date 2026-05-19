namespace game_x.domain.Entities;

public sealed class NavigationItem : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    public string Title { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public NavigationTargetType TargetType { get; private set; }
    public int? TargetId { get; private set; }
    public string CustomUrl { get; private set; } = string.Empty;
    public int? IconId { get; private set; }
    public MediaFile? Icon { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<NavigationItemTranslation> Translations { get; private set; } = [];

    public static NavigationItem Create(
        string title,
        string slug,
        NavigationTargetType targetType,
        int? targetId,
        string customUrl,
        int priority)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(slug, nameof(slug));

        if (priority < 0)
            throw new ArgumentException("Priority must be greater than or equal 0.", nameof(priority));

        if (targetType == NavigationTargetType.Category && !targetId.HasValue)
            throw new ArgumentException("TargetId is required when TargetType is Category.", nameof(targetId));

        return new NavigationItem
        {
            PublicId = Guid.CreateVersion7(),
            Title = title,
            Slug = slug.ToLowerInvariant().Trim(),
            TargetType = targetType,
            TargetId = targetId,
            CustomUrl = customUrl,
            Priority = priority,
            IsActive = true
        };
    }

    public void Update(
        string title,
        string slug,
        NavigationTargetType targetType,
        int? targetId,
        string customUrl,
        int priority,
        bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(slug, nameof(slug));

        if (priority < 0)
            throw new ArgumentException("Priority must be greater than or equal 0.", nameof(priority));

        if (targetType == NavigationTargetType.Category && !targetId.HasValue)
            throw new ArgumentException("TargetId is required when TargetType is Category.", nameof(targetId));

        if (targetType == NavigationTargetType.ExternalLink && !customUrl.IsNullOrWhiteSpace())
            throw new ArgumentException("Custom URL is required when TargetType is External Link.", nameof(targetId));

        Title = title;
        Slug = slug.ToLowerInvariant().Trim();
        TargetType = targetType;
        TargetId = targetId;
        CustomUrl = customUrl;
        Priority = priority;
        IsActive = isActive;
    }

    public void UploadIcon(MediaFile file)
    {
        IconId = file.Id;
        Icon = file;
    }

    public void UpdateStatus(bool isActive)
    {
        IsActive = isActive;
    }

    public void UpsertTranslation(LanguageCode lang, string title)
    {
        var existing = Translations.FirstOrDefault(x => x.LanguageCode.Equals(lang));

        if (existing is null)
        {
            var newTranslation = NavigationItemTranslation.Create(Id, lang, title);
            Translations.Add(newTranslation);
            return;
        }

        existing.Update(title);
    }
}
