namespace game_x.application.Features.LiveStreams.Gifts.Commands.CreateLiveStreamGift;

public sealed class CreateLiveStreamGiftHandler : ICommandHandler<CreateLiveStreamGiftCommand>
{
    public async Task<Unit> Handle(CreateLiveStreamGiftCommand request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return Unit.Value;
    }
}
