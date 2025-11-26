namespace game_x.application.Features.Accounts.Admin.Commands.CreateTalent;

public record CreateTalentCommand(string Email, string Nickname, string Password) : ICommand;
