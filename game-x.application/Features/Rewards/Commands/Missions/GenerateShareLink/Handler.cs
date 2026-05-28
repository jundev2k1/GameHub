using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.Rewards.Commands.Missions.GenerateShareLink;

public sealed class ShareLinkHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IMissionRepo missionRepo,
    IShareLinkRepo shareLinkRepo,
    IOptions<GameXSettings> settings) : ICommandHandler<ShareLinkCommand, ShareLinkResponse>
{
    private readonly string _domain = settings.Value.PublicWebUrl;
    private readonly DateTime _expiredTime = DateTime.UtcNow.AddDays(7);
    
    public async Task<ShareLinkResponse> Handle(ShareLinkCommand cmd, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var mission = await ValidateAsync(cmd, ct);

        var existing = await shareLinkRepo.GetActiveByUserAndMissionAsync(userId, cmd.MissionId, ct);

        if (existing is not null)
            return new ShareLinkResponse(BuildShareUrl(_domain, existing.Code));
        
        var code = Guid.CreateVersion7().ToString("N")[..12];

        var link = ShareLink.Create(
            userId: userId,
            missionId: mission.Id,
            code: code,
            expiredAt: _expiredTime);

        await shareLinkRepo.AddAsync(link, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return new ShareLinkResponse(BuildShareUrl(_domain, link.Code));
    }

    private async Task<Mission> ValidateAsync(ShareLinkCommand cmd, CancellationToken ct)
    {
        var mission = await missionRepo.GetByIdAsync(cmd.MissionId, ct)
            ?? throw new NotFoundException(MessageCode.Reward.MissionNotFound);

        if (!mission.IsActive)
            throw new BadRequestException(MessageCode.Reward.MissionInactive);

        if (mission.Type != MissionType.Share)
            throw new BadRequestException(MessageCode.Reward.MissionTypeInvalid);

        return mission;
    }

    private static string BuildShareUrl(string domain, string code) => $"{domain}/share/{code}";
}