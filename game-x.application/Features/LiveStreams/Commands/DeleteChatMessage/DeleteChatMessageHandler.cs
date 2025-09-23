using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Commands.DeleteChatMessage;

public sealed class DeleteChatMessageHandler(
    IUserAccessor userAccessor,
    IUnitOfWork unitOfWork,
    ILiveStreamChatRepo streamChatRepo,
    ILiveStreamHubService liveStreamHub,
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<DeleteChatMessageCommand>
{
    public async Task<Unit> Handle(DeleteChatMessageCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var streamInfo = liveStreamManager.GetLiveStreamStatus(request.StreamKey)
            ?? throw new NotFoundException("Live stream is not found.");
        if (streamInfo.AssignedTo?.Id != userId)
            throw new ForbiddenException("Only the host can delete chat messages.");

        // Delete from database
        await streamChatRepo.DeleteAsync(request.MessageId, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Remove from cache and notify clients
        liveStreamManager.RemoveMessageFromStream(request.StreamKey, request.MessageId);
        await liveStreamHub.NotifyMessageDeleted(request.StreamKey, request.MessageId);

        return Unit.Value;
    }
}
