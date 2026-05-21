namespace game_x.application.Features.Rewards.Commands.Missions.Remove;

public sealed record RemoveMissionCommand(Guid Id): ICommand<Unit>;