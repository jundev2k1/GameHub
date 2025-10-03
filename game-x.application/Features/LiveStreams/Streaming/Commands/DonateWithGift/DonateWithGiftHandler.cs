using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnLiveStreamDonated;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithGift;

public sealed class DonateWithGiftHandler(
    ILiveStreamGiftRepo liveStreamGiftRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IUserBalanceRepo userBalanceRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IUserAccessor userAccessor,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<DonateWithGiftCommand>
{
    public async Task<Unit> Handle(DonateWithGiftCommand request, CancellationToken ct = default)
    {
        var streamInfo = liveStreamManager.GetLiveStreamStatus(request.StreamKey!)
            ?? throw new NotFoundException(nameof(request.StreamKey), request.StreamKey!);

        var userId = userAccessor.GetUserId();
        var targetCrypto = await cryptoTokenRepo.GetByIdAsync(request.CryptoTokenId, ct);
        var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, targetCrypto.Id, ct)
            ?? throw new NotFoundException("User banlance not found.");
        var talentBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(streamInfo.AssignedTo!.Id, targetCrypto.Id, ct)
            ?? throw new NotFoundException("Talent balance not found.");

        var gift = await liveStreamGiftRepo.GetByIdAsync(request.GiftId, ct);
        var targetGiftPrice = gift.GiftPrices
            .FirstOrDefault(gp => gp.CryptoToken.PublicId == targetCrypto.PublicId && gp.IsActive)
            ?? throw new BadRequestException("Gift price is not available.");

        // Check if user has enough balance
        if (userBalance.Amount < targetGiftPrice.TokenCost)
            throw new BadRequestException(
                MessageCode.System.ValidateFailed,
                "Insufficient balance to send this gift.",
                new { GiftPrice = targetGiftPrice.TokenCost, CurrentBalance = userBalance.Amount });

        var @event = new OnLiveStreamDonatedEvent(
            streamInfo,
            userId,
            userBalance.PublicId,
            talentBalance.PublicId,
            targetGiftPrice.TokenCost,
            targetCrypto.Id,
            request.Message.Trim(),
            gift);
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }
}
