using game_x.application.Common.Files;
using System.Text.Json.Serialization;

namespace game_x.application.Features.BankAccountVerifications.Commands._3_ResubmitBankAccount;

public record ResubmitBankAccountCommand(
    string? BankName,
    string? BankCode,
    string? AccountName,
    string? AccountNumber,
    [property: JsonIgnore] string CurrencyCode,
    FileUpload? Image) : ICommand;
