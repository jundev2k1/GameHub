using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGift;

public sealed class UpdateLiveStreamGiftHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamGiftRepo liveStreamGiftRepo) : ICommandHandler<UpdateLiveStreamGiftCommand>
{
    public async Task<Unit> Handle(UpdateLiveStreamGiftCommand request, CancellationToken ct = default)
    {
        await liveStreamGiftRepo.UpdateAsync(request.Id, async gift =>
        {
            gift.Update(
                request.Name.Trim(),
                request.Notes?.Trim(),
                request.CoinCost,
                request.Priority);
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        return Unit.Value;
    }
}
