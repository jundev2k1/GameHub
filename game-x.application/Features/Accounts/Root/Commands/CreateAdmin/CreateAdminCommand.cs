namespace game_x.application.Features.Accounts.Root.Commands.CreateAdmin;

public record CreateAdminCommand(string Username, string Password) : ICommand;
