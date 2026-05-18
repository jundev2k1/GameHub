namespace game_x.application.Features.Rewards.Commands.RemoveCatalogItem;

public sealed record RemoveCatalogItemCommand(Guid Id): ICommand<Unit>;