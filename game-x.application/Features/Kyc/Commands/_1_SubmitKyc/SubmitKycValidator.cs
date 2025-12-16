using game_x.application.Common.Files;
using game_x.application.Extensions;

namespace game_x.application.Features.Kyc.Commands._1_SubmitKyc;

public sealed class SubmitKycValidator : AbstractValidator<SubmitKycCommand>
{
    public SubmitKycValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage($"{nameof(SubmitKycCommand.FullName)} must be not empty.")
            .MaximumLength(30).WithMessage($"{nameof(SubmitKycCommand.FullName)} must be not greater than 30 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotNull().WithMessage($"{nameof(SubmitKycCommand.DateOfBirth)} must be not null.")
            .Must(BeAValidDate).WithMessage("Invalid date of birth.")
            .Must(BeRealisticAge).WithMessage("Age must be between 16 and 120 years.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage($"{nameof(SubmitKycCommand.Address)} must be not empty.")
            .MaximumLength(100).WithMessage($"{nameof(SubmitKycCommand.Address)} must be not greater than 100 characters.");

        RuleFor(x => x.IdNumber)
            .NotEmpty().WithMessage($"{nameof(SubmitKycCommand.IdNumber)} must be not empty.")
            .MaximumLength(20).WithMessage($"{nameof(SubmitKycCommand.IdNumber)} must be not greater than 20 characters.");

        RuleFor(x => x.FrontImage)
            .Must(BeAValidFileType).WithMessage("Invalid file type.")
            .Must(BeAValidFileSize).WithMessage("File upload must be not greater than 5 MB.");

        RuleFor(x => x.BackImage)
            .Must(BeAValidFileType).WithMessage("Invalid file type.")
            .Must(BeAValidFileSize).WithMessage("File upload must be not greater than 5 MB.");
    }

    private bool BeAValidDate(DateTime dob)
    {
        return dob <= DateTime.UtcNow.Date;
    }

    private bool BeRealisticAge(DateTime dob)
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - dob.Year;

        if (dob > today.AddYears(-age)) age--;

        return age >= 16 && age <= 120;
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
