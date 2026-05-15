namespace game_x.domain.Entities;

public sealed class GameCategoryTranslation : BaseEntity<int>, IAuditable
{
    public int CategoryId { get; private set; }
    public GameCategory Category { get; private set; } = default!;
    public LanguageCode LanguageCode { get; private set; } = default!;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;

    public static GameCategoryTranslation Create(
        int categoryId,
        LanguageCode languageCode,
        string name,
        string description,
        string note)
    {
        return new GameCategoryTranslation
        {
            CategoryId = categoryId,
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
