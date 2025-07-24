namespace game_x.application.Features.BankAccountManagement.Client.Commands.DeleteBankAccount;

public record DeleteBankAccountCommand(Guid BankAccountCode) : ICommand;
