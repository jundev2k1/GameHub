using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnLiveStreamDonated;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithFiatCurrency;

public sealed class DonateWithCryptoTokensHandler(
    ICryptoTokenRepo cryptoTokenRepo,
    IUserBalanceRepo userBalanceRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IUserAccessor userAccessor,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<DonateWithCryptoTokensCommand>
{
    public async Task<Unit> Handle(DonateWithCryptoTokensCommand request, CancellationToken ct = default)
    {
        var streamInfo = liveStreamManager.GetLiveStreamStatus(request.StreamKey)
            ?? throw new NotFoundException(nameof(request.StreamKey), request.StreamKey);

        // Get user balance and talent balance
        var userId = userAccessor.GetUserId();
        var targetCrypto = await cryptoTokenRepo.GetByIdAsync(request.CryptoTokenId, ct);
        var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, targetCrypto.Id, ct)
            ?? throw new NotFoundException("User banlance not found.");
        var talentBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(streamInfo.AssignedTo!.Id, targetCrypto.Id, ct)
            ?? throw new NotFoundException("Talent balance not found.");

        // Check if user has enough balance
        if (userBalance.Amount < request.Amount)
            throw new BadRequestException(
                MessageCode.System.ValidateFailed,
                "Insufficient balance to donate.",
                new { CurrentBalance = userBalance.Amount });

        var @event = new OnLiveStreamDonatedEvent(
            streamInfo,
            userId,
            userBalance.PublicId,
            talentBalance.PublicId,
            request.Amount,
            targetCrypto.Id,
            request.Message.Trim());
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }
}
