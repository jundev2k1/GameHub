using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace game_x.domain.Entities;

public class User : IdentityUser, IEntity, IAuditable
{
    public string Nickname { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<ChainTransaction> ChainTransactions { get; set; } = [];
    public ICollection<GameTransaction> GameTransactions { get; set; } = [];
    public ICollection<UserUsdtLedger> UserUsdtLedgers { get; set; } = [];

    /// <summary>The user's balance in all currencies in the system.</summary>
    public ICollection<UserBalance> UserBalances { get; set; } = [];
    public ICollection<BalanceTransferLog> BalanceTransferLogs { get; set; } = [];

    public UserExtend? UserExtend { get; set; }
    public UserKyc? UserKyc { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<UserBankAccount> UserBankAccounts { get; set; } = [];

    public static User Create(
        string userName,
        string email,
        string nickName = "",
        string phoneNumber = "",
        string countryCode = "",
        UserStatus status = UserStatus.Active,
        List<UserRole>? userRoles = null)
    {
        if (!email.IsNullOrWhiteSpace() && !IsEmail(email))
            throw new ArgumentException("Email ({email}) wrong format.", email);

        if (!phoneNumber.IsNullOrWhiteSpace() && !IsPhoneNumber(phoneNumber))
            throw new ArgumentException("Phone number ({phoneNumber}) wrong format.", phoneNumber);

        return new User()
        {
            UserName = userName,
            Email = email,
            Nickname = nickName,
            PhoneNumber = phoneNumber,
            CountryCode = countryCode,
            Status = status,
            UserRoles = userRoles ?? [],
        };
    }

    public static bool IsEmail(string email)
    {
        if (email.IsNullOrWhiteSpace())
            return false;

        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }

    public static bool IsPhoneNumber(string phone)
    {
        if (phone.IsNullOrWhiteSpace())
            return false;

        var pattern = @"^\+?[0-9\s\-\(\)]+$";
        if (!Regex.IsMatch(phone, pattern))
            return false;

        int digitCount = phone.Count(char.IsDigit);

        return digitCount >= 7 && digitCount <= 15;
    }

    public void UpdateStatus(UserStatus status)
    {
        Status = status;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
    }

    public void ConfirmPhoneNumber()
    {
        PhoneNumberConfirmed = true;
    }

    public (bool Result, System.Enum? ErrorCode) CheckValidUser()
    {
        if (Status == UserStatus.Inactive)
            return (false, MessageCode.User.UserInvalid);
        if (IsDeleted)
            return (false, MessageCode.User.UserDisabled);

        return (true, null);
    }

    private bool Has(string roleName)
        => UserRoles.Any(r => r.Role.Name == roleName);

    public bool IsRoot => Has(AppRoles.Root);
    public bool IsAdmin => Has(AppRoles.Admin);
    public bool IsCs => Has(AppRoles.Cs);
    public bool IsUser => Has(AppRoles.User);

    public void AddUserExtend(UserExtend userExtend)
    {
        UserExtend = userExtend;
    }

    public void AddUserKyc(UserKyc kycProfile)
    {
        UserKyc = kycProfile;
    }

    public void AddUserBankAccount(UserBankAccount bankAccount)
    {
        if (UserBankAccounts.Any(uba => uba.CurrencyId.Equals(bankAccount.CurrencyId)))
            throw new ArgumentException("User already has a bank account for this currency.", nameof(bankAccount));

        UserBankAccounts.Add(bankAccount);
    }
}
