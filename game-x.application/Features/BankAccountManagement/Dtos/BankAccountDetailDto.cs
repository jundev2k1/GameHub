namespace game_x.application.Features.BankAccountManagement.Dtos;

public class BankAccountDetailDto
{
    public string BankAccountCode { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BankAccountName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;
    public DateTime CreatedAt { get; set; }
}
