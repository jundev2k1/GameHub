namespace game_x.application.Features.Games.Common.Queries.GetActiveTypes;

public record GetActiveTypesQuery : IQuery<GetActiveTypesResult[]>;

public record GetActiveTypesResult(Guid Id, string Name, string Description);
