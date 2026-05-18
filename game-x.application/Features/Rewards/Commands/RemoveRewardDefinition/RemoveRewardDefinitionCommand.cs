namespace game_x.application.Features.Rewards.Commands.RemoveRewardDefinition;

public sealed record RemoveRewardDefinitionCommand(Guid Id): ICommand<Unit>;