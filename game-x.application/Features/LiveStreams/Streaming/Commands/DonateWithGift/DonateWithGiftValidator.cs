namespace game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithGift;

public sealed class DonateWithGiftValidator : AbstractValidator<DonateWithGiftCommand>
{
    public DonateWithGiftValidator()
    {
        RuleFor(x => x.StreamKey)
            .NotEmpty().WithMessage("Stream key is required.");

        RuleFor(x => x.Message)
            .MaximumLength(256).WithMessage("Message cannot exceed 256 characters.");
    }
}
