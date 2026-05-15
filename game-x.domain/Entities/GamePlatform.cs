namespace game_x.domain.Entities;

public sealed class GamePlatform : BaseEntity<int>
{
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public bool IsActive { get; private set; } = true;
    public ICollection<GamePlatformTranslation> Translations { get; private set; } = [];
    public ICollection<GamePlatformBalance> Balances { get; private set; } = [];
    public ICollection<Game> Games { get; private set; } = [];
    public ICollection<TransactionExternal> TransactionExternals { get; private set; } = [];

    public static GamePlatform Create(string name, string desc, string note, int priority, Guid? publicId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        return new GamePlatform
        {
            PublicId = publicId ?? Guid.CreateVersion7(),
            Name = name,
            Description = desc,
            Note = note,
            Priority = priority,
        };
    }

    public void UpsertTranslation(LanguageCode lang, string name, string description, string note)
    {
        var existing = Translations.FirstOrDefault(x => x.LanguageCode.Equals(lang));

        if (existing is null)
        {
            var newTranslation = GamePlatformTranslation.Create(Id, lang, name, description, note);
            Translations.Add(newTranslation);
            return;
        }

        existing.Update(name, description, note);
    }

    public void Update(string name, string desc, string note, int priority, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        if (priority < 0)
            throw new ArgumentException("Priority must be greater than or equal 0.", nameof(priority));

        Name = name;
        Description = desc;
        Note = note;
        Priority = priority;
        IsActive = isActive;
    }

    public void SetActive(bool isActive) =>
        IsActive = isActive;
}
