using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnLiveStreamDonated;
using game_x.share.Extensions;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.DonateWithCryptoTokens;

public sealed class DonateWithCryptoTokensHandler(
    ICryptoTokenRepo cryptoTokenRepo,
    IUserBalanceRepo userBalanceRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IUserAccessor userAccessor,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<DonateWithCryptoTokensCommand>
{
    public async Task<Unit> Handle(DonateWithCryptoTokensCommand request, CancellationToken ct = default)
    {
        var streamInfo = liveStreamManager.GetLiveStreamStatus(request.StreamKey!)
            ?? throw new NotFoundException(nameof(request.StreamKey), request.StreamKey!);

        var userId = userAccessor.GetUserId();

        // Check if the user is blocked from viewing the stream
        var banInfo = streamInfo.BlackList.FirstOrDefault(
            bl => bl.UserId == userId
            && bl.Action == Dtos.BlackListAction.Donate
            && bl.BanUntil > DateTime.UtcNow);
        if (banInfo != null)
            throw new ForbiddenException(
                MessageCode.System.Forbidden,
                "You are blocked from donating this live stream.",
                new { Action = banInfo.Action.ToCamelCase(), banInfo.BanUntil, Reason = banInfo.Reason.ToCamelCase() });

        // Get user balance and talent balance
        var targetCrypto = await cryptoTokenRepo.GetByIdAsync(request.CryptoTokenId, ct);
        var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, targetCrypto.Id, ct)
            ?? throw new NotFoundException("User banlance not found.");

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
            request.Amount,
            targetCrypto.Id,
            request.Message.Trim());
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }
}
