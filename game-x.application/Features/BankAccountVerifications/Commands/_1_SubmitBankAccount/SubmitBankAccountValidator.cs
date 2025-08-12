using game_x.application.Common.Files;
using game_x.application.Extensions;

namespace game_x.application.Features.BankAccountVerifications.Commands._1_SubmitBankAccount;

public sealed class SubmitBankAccountValidator : AbstractValidator<SubmitBankAccountCommand>
{
    public SubmitBankAccountValidator()
    {
        RuleFor(x => x.BankName)
            .NotEmpty().WithMessage($"{nameof(SubmitBankAccountCommand.BankName)} must be not empty.")
            .MaximumLength(255).WithMessage($"{nameof(SubmitBankAccountCommand.BankName)} must be not greater than 255 characters.");

        RuleFor(x => x.BankCode)
            .NotEmpty().WithMessage($"{nameof(SubmitBankAccountCommand.BankCode)} must be not empty.")
            .MaximumLength(50).WithMessage($"{nameof(SubmitBankAccountCommand.BankCode)} must be not greater than 50 characters.");

        RuleFor(x => x.AccountName)
            .NotEmpty().WithMessage($"{nameof(SubmitBankAccountCommand.AccountName)} must be not empty.")
            .MaximumLength(255).WithMessage($"{nameof(SubmitBankAccountCommand.AccountName)} must be not greater than 255 characters.");
        
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage($"{nameof(SubmitBankAccountCommand.AccountNumber)} must be not empty.")
            .MaximumLength(50).WithMessage($"{nameof(SubmitBankAccountCommand.AccountNumber)} must be not greater than 50 characters.")
            .IsNumber(nameof(SubmitBankAccountCommand.AccountNumber));

        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage($"{nameof(SubmitBankAccountCommand.CurrencyCode)} must be not empty.")
            .MaximumLength(20).WithMessage($"{nameof(SubmitBankAccountCommand.CurrencyCode)} must be not greater than 20 characters.")
            .Must(CurrencyUnit.IsValid).WithMessage($"{nameof(SubmitBankAccountCommand.AccountNumber)} is invalid.");

        RuleFor(x => x.Image)
            .Must(BeAValidFileType).WithMessage("Invalid file type.")
            .Must(BeAValidFileSize).WithMessage("File upload must be not greater than 5 MB.");
    }

    private bool BeAValidFileType(FileUpload file)
    {
        var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var validMimeTypes = new[] { "image/jpeg", "image/png" };
        return validExtensions.Contains(file.Extension)
            && validMimeTypes.Contains(file.ContentType);
    }

    private bool BeAValidFileSize(FileUpload file)
    {
        var maxSize = 5 * 1080 * 1080; // 5 MB
        return file.Length < maxSize;
    }
}
