using game_x.application.Common.Files;

namespace game_x.application.Features.Interactions.Characters.Commands.CreatePoseCharacter;

public sealed class CreatePoseCharacterValidator : AbstractValidator<CreatePoseCharacterCommand>
{
    public CreatePoseCharacterValidator()
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
            .Must(BeAValidFileSize).WithMessage("File upload must be not greater than 10 MB.");
    }

    private static bool BeAValidFileType(FileUpload? file)
    {
        if (file is null) return true;

        return FileUpload.ImageExtensions.Contains(file.Extension)
            && FileUpload.ImageMimeTypes.Contains(file.ContentType);
    }

    private static bool BeAValidFileSize(FileUpload? file)
    {
        if (file is null) return true;

        return file.Length < FileUpload.ImageMaxSize;
    }
}
