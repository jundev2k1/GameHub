using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.DeleteLiveStreamGift;

public sealed class DeleteLiveStreamGiftHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamGiftRepo liveStreamGiftRepo,
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<DeleteLiveStreamGiftCommand>
{
    public async Task<Unit> Handle(DeleteLiveStreamGiftCommand request, CancellationToken ct = default)
    {
        await liveStreamGiftRepo.UpdateAsync(request.Id, async gift =>
        {
            gift.Delete();
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        await liveStreamManager.RefreshGiftCacheAsync(ct);
        return Unit.Value;
    }
}
