namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGift;

public sealed class UpdateLiveStreamGiftValidator : AbstractValidator<UpdateLiveStreamGiftCommand>
{
    public UpdateLiveStreamGiftValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(255).WithMessage("Name cannot exceed 255 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(4000).WithMessage("Notes cannot exceed 4000 characters.");

        RuleFor(x => x.CoinCost)
            .GreaterThanOrEqualTo(0).WithMessage("Coin cost must be non-negative.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage("Priority must be non-negative.");
    }
}
