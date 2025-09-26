using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.SendChatMessage;

public sealed class SendChatMessageHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IUnitOfWork unitOfWork,
    ILiveStreamChatRepo streamChatRepo,
    ILiveStreamHubService liveStreamHubService,
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<SendChatMessageCommand>
{
    public async Task<Unit> Handle(SendChatMessageCommand request, CancellationToken ct = default)
    {
        var targetUser = await userRepo.GetUserByIdAsync(userAccessor.GetUserId(), ct);
        var streamInfo = liveStreamManager.GetLiveStreamStatus(request.StreamKey)
            ?? throw new NotFoundException("Live stream is not found.");

        // Check if user is muted in this stream
        var isMute = streamInfo.BlackList.Any(bl =>
            bl.UserId == targetUser.Id
            && bl.BlockTo > DateTime.UtcNow
            && (bl.Action == BlackListAction.Chat || bl.Action == BlackListAction.View));
        if (isMute)
            throw new ForbiddenException("You are muted in this live stream.");

        // Handle create and broadcast message
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // Create chat message
            var chatMessage = LiveStreamChatMessage.Create(
                Guid.Parse(request.Id),
                streamInfo.LocalId,
                targetUser.Id,
                request.Message.Trim(),
                LiveStreamChatMessageType.UserMessage);

            await streamChatRepo.CreateAsync(chatMessage, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // Broadcast to group
            var newChatMessage = await streamChatRepo.GetByIdAsync(chatMessage.PublicId, ct);
            var chatMessageDto = newChatMessage.Adapt<LiveStreamChatMessageDto>();
            liveStreamManager.AddMessageToStream(request.StreamKey, chatMessageDto);
            await liveStreamHubService.SendChatMessage(request.StreamKey, chatMessageDto);

            await unitOfWork.CommitAsync(ct);
        }
        catch
        {
            // In case of any error, rollback and notify failure
            await unitOfWork.RollbackAsync(ct);
            await liveStreamHubService.NotifyMessageFailed(request.StreamKey, targetUser.Id, request.Id);
        }

        return Unit.Value;
    }
}
