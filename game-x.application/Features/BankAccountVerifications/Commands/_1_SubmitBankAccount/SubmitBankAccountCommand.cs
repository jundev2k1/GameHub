using game_x.application.Common.Files;

namespace game_x.application.Features.BankAccountVerifications.Commands._1_SubmitBankAccount;

public record SubmitBankAccountCommand(
    string BankName,
    string BankCode,
    string AccountName,
    string AccountNumber,
    string CurrencyCode,
    FileUpload Image) : ICommand;
