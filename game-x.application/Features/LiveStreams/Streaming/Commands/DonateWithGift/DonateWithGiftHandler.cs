namespace game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithGift;

public sealed class DonateWithGiftHandler : ICommandHandler<DonateWithGiftCommand>
{
    public async Task<Unit> Handle(DonateWithGiftCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;

        return Unit.Value;
    }
}
