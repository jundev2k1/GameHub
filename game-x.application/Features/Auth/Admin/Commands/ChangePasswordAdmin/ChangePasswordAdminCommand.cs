namespace game_x.application.Features.Auth.Admin.Commands.ChangePasswordAdmin;

public record ChangePasswordAdminCommand(string Password, string NewPassword) : ICommand;
