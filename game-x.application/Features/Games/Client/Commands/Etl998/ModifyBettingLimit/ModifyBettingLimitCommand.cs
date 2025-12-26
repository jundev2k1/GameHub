namespace game_x.application.Features.Games.Client.Commands.Etl998.ModifyBettingLimit;

public record ModifyBettingLimitCommand(int LimitId) : ICommand<bool>;