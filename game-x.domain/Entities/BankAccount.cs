namespace game_x.domain.Entities;

public sealed class BankAccount : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public string BankAccountNumber { get; private set; } = string.Empty;
    public string BankAccountName { get; private set; } = string.Empty;
    public string BankName { get; private set; } = string.Empty;
    public string BranchName { get; private set; } = string.Empty;
    public CurrencyUnit CurrencyCode { get; private set; } = default!;
    public AccountType AccountType { get; private set; } = default!;
    public AccountStatus Status { get; private set; } = default!;
    public string OwnerId { get; private set; } = string.Empty;
    public AppUser Owner { get; private set; } = default!;
    public bool IsDefault { get; private set; }
    public bool IsExternal { get; private set; }

    public static BankAccount Create(
        string accountNumber,
        string name,
        string bankName,
        string branchName,
        CurrencyUnit currencyCode,
        AccountType type,
        AccountStatus status,
        string ownerId,
        bool isDefault = false,
        bool isExternal = false)
    {
        var bankAccount = new BankAccount
        {
            BankAccountNumber = accountNumber,
            BankAccountName = name,
            BankName = bankName,
            BranchName = branchName,
            CurrencyCode = currencyCode,
            AccountType = type,
            Status = status,
            OwnerId = ownerId,
            IsDefault = isDefault,
            IsExternal = isExternal,

        };
        return bankAccount;
    }
    public void Update(

          string accountNumber,
          string name,
          string branchName,
          string bankName,
          CurrencyUnit currencyCode,
          AccountType type,
          AccountStatus status

          )
    {
        BankAccountNumber = accountNumber;
        BankAccountName = name;
        BankName = bankName;
        BranchName = branchName;
        CurrencyCode = currencyCode;
        AccountType = type;
        Status = status;




    }

    public void UpdateIsDefault(bool isDefault) => IsDefault = isDefault;
    public void UpdateIsDefault(Guid bankAccountCode) => IsDefault = PublicId == bankAccountCode;
}
