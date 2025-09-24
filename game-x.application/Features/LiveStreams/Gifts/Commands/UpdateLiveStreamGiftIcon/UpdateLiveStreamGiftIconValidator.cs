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
            .Must(BeAValidIcon).WithMessage("Icon must be a valid image file (JPEG, PNG, WEBP) and not exceed 5 MB.");
    }

    private bool BeAValidIcon(FileUpload fileUpload)
    {
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        const long maxFileSize = 5 * 1024 * 1024; // 5 MB

        return fileUpload != null
            && allowedTypes.Contains(fileUpload.ContentType)
            && fileUpload.Length <= maxFileSize;
    }
}
