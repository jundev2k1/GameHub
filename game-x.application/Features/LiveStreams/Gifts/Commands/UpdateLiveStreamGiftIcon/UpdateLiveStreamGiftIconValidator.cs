using game_x.application.Common.Files;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftIcon;

public sealed class UpdateLiveStreamGiftIconValidator : AbstractValidator<UpdateLiveStreamGiftIconCommand>
{
    public UpdateLiveStreamGiftIconValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("GiftId is required.");

        RuleFor(x => x.FileUpload)
            .NotNull().WithMessage("Icon file is required.")
            .Must(BeAValidIcon).WithMessage("Icon must be a valid image file (JPEG, PNG, WEBP) and not exceed 10 MB.");
    }

    private bool BeAValidIcon(FileUpload fileUpload)
    {
        return fileUpload != null
            && FileUpload.ImageMimeTypes.Contains(fileUpload.ContentType)
            && fileUpload.Length <= FileUpload.ImageMaxSize;
    }
}
