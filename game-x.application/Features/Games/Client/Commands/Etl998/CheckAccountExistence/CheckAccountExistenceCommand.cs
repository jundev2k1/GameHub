namespace game_x.application.Features.Games.Client.Commands.Etl998.CheckAccountExistence;

public record CheckAccountExistenceCommand(string AccountName) : ICommand<bool>;