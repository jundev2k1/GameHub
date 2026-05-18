namespace game_x.application.Features.Games.Common.Queries.GetActivePlatforms;

public record GetActivePlatformsQuery : IQuery<GetActivePlatformsResult[]>;

public record GetActivePlatformsResult(Guid Id, string Name, string Description);
