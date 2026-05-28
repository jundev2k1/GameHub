using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameRecommendDetail;

public record GetGameRecommendDetailQuery(Guid Id) : IQuery<GetGameRecommendDetailDto>;

public class GetGameRecommendDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? BannerId { get; set; }
    public RecommendationType Type { get; set; }
    public PublishStatus Status { get; set; } = PublishStatus.Draft;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public GameRecommendListItemDto[] Items { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
