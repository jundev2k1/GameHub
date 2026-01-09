using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Remainders.Commands.SubscribeStream;

public sealed class SubscribeStreamHandler(
    IUserAccessor userAccessor,
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamRemainderRepo streamRemainderRepo) : ICommandHandler<SubscribeStreamCommand>
{
    public async Task<Unit> Handle(SubscribeStreamCommand request, CancellationToken ct = default)
    {
        var targetStream = await liveStreamRepo.GetByStreamKeyAsync(request.StreamKey!, ct);
        if (targetStream.Status != LiveStreamStatus.Scheduled)
            throw new BadRequestException(MessageCode.System.InvalidResourceState);

        var userId = userAccessor.GetUserId();
        var remainders = request.Channels
            .Select(channel => LiveStreamReminder.Create(targetStream.Id, userId, channel));

        await unitOfWork.WithTransactionAsync(async () =>
        {
            await streamRemainderRepo.CreateRangeAsync(remainders, ct);
        }, ct);

        return Unit.Value;
    }
}
