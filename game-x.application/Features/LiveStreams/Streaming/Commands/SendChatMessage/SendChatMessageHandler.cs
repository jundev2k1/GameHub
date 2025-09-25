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
        var streamSetting = liveStreamManager.GetLiveStreamStatus(request.StreamKey)
            ?? throw new NotFoundException("Live stream is not found.");
        if (!streamSetting.IsLive)
            throw new ForbiddenException("Live stream is offline.");

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var chatMessage = LiveStreamChatMessage.Create(
                Guid.Parse(request.Id),
                streamSetting.LocalId,
                targetUser.Id,
                request.Message.Trim(),
                LiveStreamChatMessageType.UserMessage);

            await streamChatRepo.CreateAsync(chatMessage, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var newChatMessage = await streamChatRepo.GetByIdAsync(chatMessage.PublicId, ct);
            var chatMessageDto = newChatMessage.Adapt<LiveStreamChatMessageDto>();
            liveStreamManager.AddMessageToStream(request.StreamKey, chatMessageDto);
            await liveStreamHubService.SendChatMessage(request.StreamKey, chatMessageDto);

            await unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            await liveStreamHubService.NotifyMessageFailed(request.StreamKey, targetUser.Id, request.Id);
        }

        return Unit.Value;
    }
}
