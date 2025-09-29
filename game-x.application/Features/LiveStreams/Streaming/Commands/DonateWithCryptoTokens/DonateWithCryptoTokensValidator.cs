using game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithFiatCurrency;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithCryptoTokens;

public sealed class DonateWithCryptoTokensValidator : AbstractValidator<DonateWithCryptoTokensCommand>
{
    public DonateWithCryptoTokensValidator()
    {
        RuleFor(x => x.StreamKey)
            .NotEmpty().WithMessage("Stream key is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Message)
            .MaximumLength(256).WithMessage("Message cannot exceed 256 characters.");
    }
}
