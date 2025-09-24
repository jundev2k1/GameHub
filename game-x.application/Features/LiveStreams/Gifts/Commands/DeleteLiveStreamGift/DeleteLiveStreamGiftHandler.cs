namespace game_x.application.Features.LiveStreams.Gifts.Commands.DeleteLiveStreamGift;

public sealed class DeleteLiveStreamGiftHandler : ICommandHandler<DeleteLiveStreamGiftCommand>
{
    public async Task<Unit> Handle(DeleteLiveStreamGiftCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}
