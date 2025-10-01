using game_x.application.Common.Files;

namespace game_x.application.Features.Interactions.Characters.Commands.UpdateDefaultPoseCharacter;

public sealed class UpdateDefaultPoseCharacterValidator : AbstractValidator<UpdateDefaultPoseCharacterCommand>
{
    public UpdateDefaultPoseCharacterValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage($"{nameof(UpdateDefaultPoseCharacterCommand.Id)} is required.");

        RuleFor(x => x.File)
            .Must(BeAValidFileType).WithMessage("Invalid file type.")
            .Must(BeAValidFileSize).WithMessage("File upload must be not greater than 10 MB.");
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
        var maxSize = 10 * 1080 * 1080; // 10 MB
        return file.Length < maxSize;
    }
}
