namespace game_x.application.Features.Games.Client.Commands.Etl998.ChangePassword;

public record ChangePasswordCommand(
    string AccountName, 
    string Password) : ICommand<bool>;