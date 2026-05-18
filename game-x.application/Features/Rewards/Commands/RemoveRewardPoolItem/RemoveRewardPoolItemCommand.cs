namespace game_x.application.Features.Rewards.Commands.RemoveRewardPoolItem;

public sealed record RemoveRewardPoolItemCommand(Guid Id): ICommand<Unit>;