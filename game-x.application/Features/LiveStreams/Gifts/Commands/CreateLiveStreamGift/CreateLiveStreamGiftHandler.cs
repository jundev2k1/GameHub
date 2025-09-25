using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.CreateLiveStreamGift;

public sealed class CreateLiveStreamGiftHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamGiftRepo liveStreamGiftRepo,
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<CreateLiveStreamGiftCommand>
{
    public async Task<Unit> Handle(CreateLiveStreamGiftCommand request, CancellationToken ct = default)
    {
        var newGift = LiveStreamGift.Create(
            request.Name.Trim(),
            request.Notes?.Trim(),
            request.Priority,
            []);
        await liveStreamGiftRepo.CreateAsync(newGift, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await liveStreamManager.RefreshGiftCacheAsync(ct);
        return Unit.Value;
    }
}
