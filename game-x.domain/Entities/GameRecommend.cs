namespace game_x.domain.Entities;

public sealed class GameRecommend : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public int? BannerId { get; private set; }
    public MediaFile? Banner { get; private set; }

    public PublishStatus Status { get; private set; } = PublishStatus.Draft;
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public ICollection<GameRecommendItem> Items { get; private set; } = [];

    public static GameRecommend Create(
        string name,
        string? description = null,
        PublishStatus status = PublishStatus.Draft,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        return new GameRecommend
        {
            Name = name,
            Description = description,
            Status = status,
            StartDate = startDate,
            EndDate = endDate
        };
    }

    public void Update(
        string name,
        string? description,
        DateTime? startDate,
        DateTime? endDate)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void UpdateBanner(MediaFile? banner)
    {
        Banner = banner;
    }

    public void SetStatus(PublishStatus status) => Status = status;

    public void AddGame(GameRecommendItem game)
    {
        ArgumentNullException.ThrowIfNull(game);

        Items.Add(game);
    }

    public void UpdateGame(ICollection<GameRecommendItem> games)
    {
        var isDuplicate = games.Select(g => g.GameId).Distinct().Count() != games.Count;
        if (isDuplicate) throw new ArgumentException("Duplicate game items.");

        Items = games;
    }
}
