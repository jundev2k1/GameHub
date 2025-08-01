namespace game_x.application.Features.Auth.Client.Commands.ChangePasswordUser;

public record ChangePasswordUserCommand(string Token, string OldPassword, string NewPassword) : ICommand;
