namespace game_x.application.Features.Games.Client.Commands.Etl998.ModifyBettingLimit;

public record ModifyBettingLimitCommand(string Tables) : ICommand<bool>;