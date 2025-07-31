using game_x.application.Common.Files;
using game_x.application.Features.Kyc.Commands._1_SubmitKyc;

namespace game_x.application.Features.Kyc.Commands._3_ResubmitKyc;

public sealed class ResubmitKycValidator : AbstractValidator<ResubmitKycCommand>
{
    public ResubmitKycValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage($"{nameof(SubmitKycCommand.FullName)} must be not empty.")
            .MaximumLength(30).WithMessage($"{nameof(SubmitKycCommand.FullName)} must be not greater than 30 characters.")
            .When(x => x.FullName is not null);

        RuleFor(x => x.DateOfBirth)
            .Must(BeAValidDate).WithMessage("Invalid date of birth.")
            .Must(BeRealisticAge).WithMessage("Age must be between 16 and 120 years.")
            .When(x => x.DateOfBirth is not null);

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage($"{nameof(SubmitKycCommand.Address)} must be not empty.")
            .MaximumLength(100).WithMessage($"{nameof(SubmitKycCommand.Address)} must be not greater than 100 characters.")
            .When(x => x.Address is not null);

        RuleFor(x => x.IdNumber)
            .NotEmpty().WithMessage($"{nameof(SubmitKycCommand.IdNumber)} must be not empty.")
            .MaximumLength(20).WithMessage($"{nameof(SubmitKycCommand.IdNumber)} must be not greater than 20 characters.")
            .When(x => x.IdNumber is not null);

        RuleFor(x => x.FrontImage)
            .Must(BeAValidFileType).WithMessage("Invalid file type.")
            .Must(BeAValidFileSize).WithMessage("File upload must be not greater than 5 MB.")
            .When(x => x.FrontImage is not null);

        RuleFor(x => x.BackImage)
            .Must(BeAValidFileType).WithMessage("Invalid file type.")
            .Must(BeAValidFileSize).WithMessage("File upload must be not greater than 5 MB.")
            .When(x => x.BackImage is not null);
    }

    private static bool BeAValidDate(DateTime? dob)
    {
        return dob is null || dob <= DateTime.UtcNow.Date;
    }

    private static bool BeRealisticAge(DateTime? dob)
    {
        if (dob is null) return true;

        var today = DateTime.UtcNow.Date;
        var age = today.Year - dob.Value.Year;

        if (dob > today.AddYears(-age)) age--;

        return age >= 16 && age <= 120;
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

        var maxSize = 5 * 1080 * 1080; // 5 MB
        return file.Length < maxSize;
    }
}
