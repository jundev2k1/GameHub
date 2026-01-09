namespace game_x.application.Features.LiveStreams.Remainders.Commands.SubscribeStream;

public sealed class SubscribeStreamValidator : AbstractValidator<SubscribeStreamCommand>
{
    public SubscribeStreamValidator()
    {
        RuleFor(x => x.Channels)
            .NotEmpty().WithMessage("Channel list are required.");
    }
}
