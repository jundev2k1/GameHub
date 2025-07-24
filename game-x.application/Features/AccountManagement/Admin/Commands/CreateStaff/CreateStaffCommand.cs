namespace game_x.application.Features.AccountManagement.Admin.Commands.CreateStaff;

public record CreateStaffCommand(string Username, string Password) : ICommand;
