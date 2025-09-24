namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGift;

public sealed class UpdateLiveStreamGiftHandler : ICommandHandler<UpdateLiveStreamGiftCommand>
{
    public async Task<Unit> Handle(UpdateLiveStreamGiftCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}
