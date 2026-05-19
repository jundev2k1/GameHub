namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.Remove;

public sealed record RemoveRewardPoolItemCommand(Guid Id): ICommand<Unit>;