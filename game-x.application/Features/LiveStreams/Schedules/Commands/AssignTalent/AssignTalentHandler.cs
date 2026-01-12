using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.LiveStreams.Schedules.Commands.AssignTalent;

public sealed class AssignTalentHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IFileManagerCacheService fileManagerCache) : ICommandHandler<AssignTalentCommand, UserSummaryInfo>
{
    public async Task<UserSummaryInfo> Handle(AssignTalentCommand request, CancellationToken ct = default)
    {
        User? talent = null;
        var streamKey = string.Empty;
        await liveStreamRepo.UpdateAsync(request.Id, async livestream =>
        {
            // Check overlapping times before assigning talent
            await CheckOverlapTime(request.TalentId, livestream.StartTime, livestream.EndTime);

            // Check if valid status, only allow Scheduled status
            if (livestream.Status != LiveStreamStatus.Scheduled)
                throw new BadRequestException("Only scheduled livestream can be assigned talent.");

            talent = await userRepo.GetUserByIdAsync(request.TalentId, ct);
            if (!talent.IsTalent) throw new BadRequestException($"This user is not a Talent.");

            livestream.AssignStream(talent.Id);

            await unitOfWork.SaveChangesAsync(ct);

            streamKey = livestream.StreamKey;
        }, ct);

        var result = talent.Adapt<UserSummaryInfo>();
        if (talent!.Avatar != null)
        {
            var avatarInfo = await fileManagerCache.GetFileInfo(talent.Avatar, ct);
            result.AvatarId = talent.AvatarId;
            result.Avatar = avatarInfo?.Url;
        }

        // Update stream cache
        var targetStream = liveStreamManager.GetLiveStreamStatus(streamKey);
        targetStream!.AssignedTo = result;
        liveStreamManager.UpdateStreamInfo(targetStream);

        return result;
    }

    private async Task CheckOverlapTime(string talentId, DateTime startTime, DateTime endTime)
    {
        var streams = await liveStreamRepo.GetsByTalentIdAsync(talentId);
        foreach (var stream in streams)
        {
            var isOverlaps = stream.Status is not (LiveStreamStatus.Cancelled or LiveStreamStatus.Ended)
                && startTime <= stream.EndTime && endTime >= stream.StartTime;
            if (isOverlaps)
            {
                throw new BadRequestException(
                    MessageCode.System.ValidateFailed,
                    new
                    {
                        IsOverlapTime = true,
                        stream.Title,
                        stream.StartTime,
                        stream.EndTime
                    });
            }
        }
    }
}
