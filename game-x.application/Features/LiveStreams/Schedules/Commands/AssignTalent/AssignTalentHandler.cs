using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.LiveStreams.Schedules.Commands.AssignTalent;

public sealed class AssignTalentHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    ILiveStreamRepo liveStreamRepo,
    IFileManagerCacheService fileManagerCache) : ICommandHandler<AssignTalentCommand, UserSummaryInfo>
{
    public async Task<UserSummaryInfo> Handle(AssignTalentCommand request, CancellationToken ct = default)
    {
        User? talent = null;
        await liveStreamRepo.UpdateAsync(request.Id, async livestream =>
        {
            // Check overlapping times before assigning talent
            await CheckOverlapTime(request.TalentId, livestream.StartTime, livestream.EndTime);

            // Check if valid status, only allow Scheduled status
            if (livestream.Status != LiveStreamStatus.Scheduled)
                throw new BadRequestException("Only scheduled livestream can be assigned talent.");

            talent = await userRepo.GetUserByIdAsync(request.TalentId, ct);
            if (talent.UserKyc == null || talent.UserKyc.Status != KycStatus.Approved)
                throw new BadRequestException("User must be kyc verified.");

            if (talent.UserBankAccounts.Count == 0
                || !talent.UserBankAccounts.Any(ba => ba.Status == UserBankAccountStatus.Approved))
                throw new BadRequestException("Must have at least 1 verified bank account");

            livestream.AssignStream(talent.Id);

            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        var result = talent.Adapt<UserSummaryInfo>();
        if (talent!.Avatar != null)
        {
            var avatarInfo = await fileManagerCache.GetFileInfo(talent.Avatar, ct);
            result.AvatarId = talent.AvatarId;
            result.Avatar = avatarInfo?.Url;
        }

        return result;
    }

    private async Task CheckOverlapTime(string talentId, DateTime startTime, DateTime endTime)
    {
        var streams = await liveStreamRepo.GetsByTalentIdAsync(talentId);
        foreach (var stream in streams)
        {
            var isOverlaps = startTime <= stream.EndTime && endTime >= stream.StartTime;
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
