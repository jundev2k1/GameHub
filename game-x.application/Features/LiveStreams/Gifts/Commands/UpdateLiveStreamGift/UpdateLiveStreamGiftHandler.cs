using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGift;

public sealed class UpdateLiveStreamGiftHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamGiftRepo liveStreamGiftRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    ICryptoTokenRepo cryptoTokenRepo) : ICommandHandler<UpdateLiveStreamGiftCommand>
{
    public async Task<Unit> Handle(UpdateLiveStreamGiftCommand request, CancellationToken ct = default)
    {
        var targetTokenIds = request.GiftPrices
            .Select(gp => gp.CryptoTokenId)
            .ToArray();
        if (targetTokenIds.Distinct().Count() != request.GiftPrices.Length)
            throw new BadRequestException("Gift prices is duplicated.");

        var cryptoTokens = await cryptoTokenRepo.GetByIdsAsync(targetTokenIds, ct);
        if (request.GiftPrices.Length != cryptoTokens.Length)
            throw new BadRequestException("Gift prices invalid.");

        await liveStreamGiftRepo.UpdateAsync(request.Id, async gift =>
        {
            var giftPrices = request.GiftPrices
                .Select(gp =>
                {
                    var targetToken = cryptoTokens.FirstOrDefault(ct => ct.PublicId == gp.CryptoTokenId)
                        ?? throw new NotFoundException("Crypto token not found.");
                    return LiveStreamGiftPrice.Create(gift.Id, targetToken.Id, gp.TokenCost, gp.IsActive);
                })
                .ToList();

            gift.Update(
                request.Name.Trim(),
                request.Notes?.Trim(),
                request.Priority,
                giftPrices);
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        await liveStreamManager.RefreshGiftCacheAsync(ct);
        return Unit.Value;
    }
}
