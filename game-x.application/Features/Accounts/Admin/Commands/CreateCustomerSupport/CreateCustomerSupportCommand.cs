namespace game_x.application.Features.Accounts.Admin.Commands.CreateCustomerSupport;

public record CreateCustomerSupportCommand(string Email, string Password) : ICommand;