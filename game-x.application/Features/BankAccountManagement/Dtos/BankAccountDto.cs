namespace game_x.application.Features.BankAccountManagement.Dtos;

public class BankAccountDto
{
    public string BankAccountCode { get; set; } = string.Empty;

    public string BankAccountNumber { get; set; } = default!;
    public string BankAccountName { get; set; } = string.Empty;

    public string BankName { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = default!;
    public string AccountType { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string OwnerId { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;

}