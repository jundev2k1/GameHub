using game_x.application.Extensions;

namespace game_x.application.Features.BankAccountManagement.Client.Commands.CreateBankAccount;

public sealed class CreateBankAccountValidator : AbstractValidator<CreateBankAccountCommand>
{
    public CreateBankAccountValidator()
    {
        RuleFor(ba => ba.BankAccountNumber)
            .NotEmpty().WithMessage("BankAccountNumber is required.")
            .IsNumber(nameof(BankAccount.BankAccountNumber));

        RuleFor(ba => ba.BankAccountName)
            .NotEmpty().WithMessage("BankAccountName is required.");

        RuleFor(ba => ba.BankName)
            .NotEmpty().WithMessage("BankName is required.");

        RuleFor(ba => ba.BranchName)
            .NotEmpty().WithMessage("BranchName is required.");

        RuleFor(ba => ba.CurrencyCode)
            .NotEmpty().WithMessage("CurrencyCode is required.")
            .Must(CurrencyUnit.IsValid).WithMessage("CurrencyCode is invalid.");
        
        RuleFor(ba => ba.AccountType)
            .NotEmpty().WithMessage("AccountType is required.")
            .Must(AccountType.IsValid).WithMessage("AccountType is invalid.");
        
        RuleFor(ba => ba.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(AccountStatus.IsValid).WithMessage("Status is invalid.");

    }
   
}
