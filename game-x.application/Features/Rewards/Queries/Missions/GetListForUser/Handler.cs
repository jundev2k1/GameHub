using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Features.Rewards.Queries.Missions.GetListForUser;

public sealed class GetMissionListForUserHandler(
    IUserAccessor userAccessor,
    IMissionRepo repo,
    IFileManagerCacheService storage) : IQueryHandler<GetMissionListForUserQuery, IReadOnlyList<MissionUserDto>>
{
    public async Task<IReadOnlyList<MissionUserDto>> Handle(GetMissionListForUserQuery request, CancellationToken ct = default)
    {
        string userId = userAccessor.GetUserId();
        var missions = await repo.GetAllForUserAsync(userId, request.Type, ct);
        var result = Task.WhenAll(
            missions.Select(async mission =>
            {
                var missionDto = mission.Adapt<MissionUserDto>();
                missionDto.MissionRewards = await Task.WhenAll(
                    mission.MissionRewards.Select(async reward =>
                    {
                        var rewardDto = reward.Adapt<MissionRewardUserDto>();
                        rewardDto.ItemIconUrl = reward.ItemIcon is null
                            ? null
                            : await storage.GetFileUrl(reward.ItemIcon, ct);
                        return rewardDto;
                    })
                );
                return missionDto;
            }));
        
        return await result;
    }
}