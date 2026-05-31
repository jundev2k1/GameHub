using game_x.application.Common.Files;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftAnimation;

public sealed class UpdateLiveStreamGiftAnimationValidator : AbstractValidator<UpdateLiveStreamGiftAnimationCommand>
{
    public UpdateLiveStreamGiftAnimationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("GiftId is required.");

        RuleFor(x => x.FileUpload)
            .NotNull().WithMessage("Animation file is required.")
            .Must(BeAValidIcon).WithMessage("Invalid animation file. Allowed types are JPEG, PNG, WEBP and max size is 10MB.")
            .When(x => x.FileUpload != null);
    }

    private bool BeAValidIcon(FileUpload? fileUpload)
    {
        if (fileUpload is null) return true;

        return FileUpload.ImageMimeTypes.Contains(fileUpload.ContentType)
            && fileUpload.Length <= FileUpload.ImageMaxSize;
    }
}
