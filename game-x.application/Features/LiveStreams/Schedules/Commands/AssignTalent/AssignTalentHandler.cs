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
            var avatarInfo = await fileManagerCache.GetFileUrl(talent.Avatar, ct);
            result.Avatar = avatarInfo?.Url;
        }

        return result;
    }
}
