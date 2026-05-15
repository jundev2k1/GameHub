namespace game_x.domain.Entities;

public sealed class GameTagTranslation : BaseEntity<int>, IAuditable
{
    public int TagId { get; private set; }
    public GameTag Tag { get; private set; } = default!;
    public LanguageCode LanguageCode { get; private set; } = default!;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;

    public static GameTagTranslation Create(int tagId, LanguageCode languageCode, string name, string description, string note)
    {
        return new GameTagTranslation
        {
            TagId = tagId,
            LanguageCode = languageCode,
            Name = name,
            Description = description,
            Note = note,
        };
    }

    public void Update(string name, string description, string note)
    {
        Name = name;
        Description = description;
        Note = note;
    }
}
