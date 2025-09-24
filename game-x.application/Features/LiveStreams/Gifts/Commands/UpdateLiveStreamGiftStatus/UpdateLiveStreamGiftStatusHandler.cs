namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftStatus;

public sealed class UpdateLiveStreamGiftStatusHandler : ICommandHandler<UpdateLiveStreamGiftStatusCommand>
{
    public async Task<Unit> Handle(UpdateLiveStreamGiftStatusCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}
