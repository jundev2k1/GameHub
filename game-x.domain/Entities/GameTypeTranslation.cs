namespace game_x.domain.Entities;

public sealed class GameTypeTranslation : BaseEntity<int>, IAuditable
{
    public int TypeId { get; private set; }
    public GameType Type { get; private set; } = default!;
    public LanguageCode LanguageCode { get; private set; } = default!;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;

    public static GameTypeTranslation Create(int typeId, LanguageCode languageCode, string name, string description, string note)
    {
        return new GameTypeTranslation
        {
            TypeId = typeId,
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
