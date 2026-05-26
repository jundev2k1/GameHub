namespace game_x.domain.Entities;

public sealed class GameMedia : BaseEntity<int>
{
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    public int GameId { get; private set; }
    public Game Game { get; private set; } = default!;
    public GameMediaType Type { get; private set; }
    public GameMediaCategory Category { get; private set; }
    public int FileId { get; private set; }
    public MediaFile File { get; private set; } = default!;
    public string? Title { get; private set; }
    public string? Note { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; }

    public static GameMedia Create(
        int gameId,
        int fileId,
        GameMediaType type,
        GameMediaCategory category,
        string title,
        string? note,
        int priority)
    {
        return new GameMedia
        {
            PublicId = Guid.CreateVersion7(),
            GameId = gameId,
            FileId = fileId,
            Type = type,
            Category = category,
            Title = title,
            Note = note,
            Priority = priority,
            IsActive = true,
        };
    }
}
