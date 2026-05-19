namespace game_x.application.Features.Rewards.Commands.CatalogItems.Create.Remove;

public sealed record CatalogItemRemoveCommand(Guid Id): ICommand<Unit>;