using System.Text.Json.Serialization;

namespace game_x.application.Features.BankAccountManagement.Client.Commands.UpdateBankAccount;

public class UpdateBankAccountCommand : ICommand
{
    [JsonIgnore]
    public Guid BankAccountCode { get; set; }

    public string? BankAccountNumber { get; set; }
    public string? BankAccountName { get; set; }
    public string? BankName { get; set; }
    public string? BranchName { get; set; }
    public string? CurrencyCode { get; set; }
    public string? AccountType { get; set; }
    public string? Status { get; set; }
}

