namespace game_x.application.Features.Games.Common.Queries.GetActiveCategories;

public record GetActiveCategoriesQuery : IQuery<GetActiveCategoriesResult[]>;

public record GetActiveCategoriesResult(Guid Id, string Name, string Description);
