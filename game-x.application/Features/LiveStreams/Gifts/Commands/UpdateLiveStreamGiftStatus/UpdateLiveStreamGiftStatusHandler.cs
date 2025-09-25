using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftStatus;

public sealed class UpdateLiveStreamGiftStatusHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamGiftRepo liveStreamGiftRepo,
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<UpdateLiveStreamGiftStatusCommand>
{
    public async Task<Unit> Handle(UpdateLiveStreamGiftStatusCommand request, CancellationToken ct = default)
    {
        // Soft delete gift
        await liveStreamGiftRepo.UpdateAsync(request.Id, async gift =>
        {
            if (request.IsActive)
            {
                gift.Enable();
            }
            else
            {
                gift.Disable();
            }

            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        // Refresh cache
        await liveStreamManager.RefreshGiftCacheAsync(ct);

        return Unit.Value;
    }
}
