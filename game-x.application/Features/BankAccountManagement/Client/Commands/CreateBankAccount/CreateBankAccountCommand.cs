namespace game_x.application.Features.BankAccountManagement.Client.Commands.CreateBankAccount;

public record CreateBankAccountCommand(
   // Guid bankAccountCode,
    string BankAccountNumber,
    string BankAccountName,
    string BankName,
    string BranchName,
    string CurrencyCode,
    string AccountType,
    string Status) : ICommand;
