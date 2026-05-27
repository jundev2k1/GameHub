using game_x.application.Common.Files;

namespace game_x.application.Features.Interactions.Characters.Commands.UpdateDefaultPoseCharacter;

public sealed class UpdateDefaultPoseCharacterValidator : AbstractValidator<UpdateDefaultPoseCharacterCommand>
{
    public UpdateDefaultPoseCharacterValidator()
    {
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
