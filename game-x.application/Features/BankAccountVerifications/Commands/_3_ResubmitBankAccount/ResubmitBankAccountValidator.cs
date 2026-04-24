using game_x.application.Common.Files;
using game_x.application.Extensions;

namespace game_x.application.Features.BankAccountVerifications.Commands._3_ResubmitBankAccount;

public sealed class ResubmitBankAccountValidator : AbstractValidator<ResubmitBankAccountCommand>
{
    public ResubmitBankAccountValidator()
    {
        RuleFor(x => x.BankName)
            .NotEmpty().WithMessage($"{nameof(ResubmitBankAccountCommand.BankName)} must be not empty.")
            .MaximumLength(255).WithMessage($"{nameof(ResubmitBankAccountCommand.BankName)} must be not greater than 255 characters.")
            .When(x => x.BankName is not null);

        RuleFor(x => x.BankCode)
            .NotEmpty().WithMessage($"{nameof(ResubmitBankAccountCommand.BankCode)} must be not empty.")
            .MaximumLength(50).WithMessage($"{nameof(ResubmitBankAccountCommand.BankCode)} must be not greater than 50 characters.")
            .When(x => x.BankCode is not null);

        RuleFor(x => x.AccountName)
            .NotEmpty().WithMessage($"{nameof(ResubmitBankAccountCommand.AccountName)} must be not empty.")
            .MaximumLength(255).WithMessage($"{nameof(ResubmitBankAccountCommand.AccountName)} must be not greater than 255 characters.")
            .When(x => x.AccountName is not null);

        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage($"{nameof(ResubmitBankAccountCommand.AccountNumber)} must be not empty.")
            .MaximumLength(50).WithMessage($"{nameof(ResubmitBankAccountCommand.AccountNumber)} must be not greater than 50 characters.")
            .IsNumber(nameof(ResubmitBankAccountCommand.AccountNumber))
            .When(x => x.AccountNumber is not null);

        RuleFor(x => x.Image)
            .Must(BeAValidFileType).WithMessage("Invalid file type.")
            .Must(BeAValidFileSize).WithMessage("File upload must be not greater than 10 MB.");
    }

    private bool BeAValidFileType(FileUpload? file)
    {
        if (file is null) return true;

        var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var validMimeTypes = new[] { "image/jpeg", "image/png" };
        return validExtensions.Contains(file.Extension)
            && validMimeTypes.Contains(file.ContentType);
    }

    private bool BeAValidFileSize(FileUpload? file)
    {
        if (file is null) return true;

        var maxSize = 10 * 1024 * 1024; // 10 MB
        return file.Length < maxSize;
    }
}
