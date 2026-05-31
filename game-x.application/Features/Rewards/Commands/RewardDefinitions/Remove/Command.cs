namespace game_x.application.Features.Rewards.Commands.RewardDefinitions.Remove;

public sealed record RemoveRewardDefinitionCommand(Guid Id): ICommand<Unit>;