namespace game_x.domain.Entities;

public sealed class GameTranslation : BaseEntity<int>, IAuditable
{
    public int GameId { get; private set; }
    public Game Game { get; private set; } = default!;
    public LanguageCode LanguageCode { get; private set; } = default!;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;

    public static GameTranslation Create(int gameId, LanguageCode languageCode, string name, string description, string note)
    {
        return new GameTranslation
        {
            GameId = gameId,
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
