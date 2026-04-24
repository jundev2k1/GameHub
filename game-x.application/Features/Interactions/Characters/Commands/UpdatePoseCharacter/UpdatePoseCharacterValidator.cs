using game_x.application.Common.Files;

namespace game_x.application.Features.Interactions.Characters.Commands.UpdatePoseCharacter;

public sealed class UpdatePoseCharacterValidator : AbstractValidator<UpdatePoseCharacterCommand>
{
    public UpdatePoseCharacterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description cannot be empty.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Notes)
            .NotEmpty().WithMessage("Notes cannot be empty.")
            .MaximumLength(4000).WithMessage("Notes cannot exceed 4000 characters.");

        RuleFor(x => x.File)
            .Must(BeAValidFileType).WithMessage("Invalid file type.")
            .Must(BeAValidFileSize).WithMessage("File upload must be not greater than 10 MB.")
            .When(x => x.File is not null);
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
