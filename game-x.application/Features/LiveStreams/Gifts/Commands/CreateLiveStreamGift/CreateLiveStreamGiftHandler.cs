using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.CreateLiveStreamGift;

public sealed class CreateLiveStreamGiftHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamGiftRepo liveStreamGiftRepo) : ICommandHandler<CreateLiveStreamGiftCommand>
{
    public async Task<Unit> Handle(CreateLiveStreamGiftCommand request, CancellationToken ct = default)
    {
        var newGift = LiveStreamGift.Create(
            request.Name.Trim(),
            request.Notes?.Trim(),
            request.CoinCost,
            request.Priority);
        await liveStreamGiftRepo.CreateAsync(newGift, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
