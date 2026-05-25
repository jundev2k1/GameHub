using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Features.Rewards.Queries.Missions.GetByUser;

public sealed class GetMissionDetailByUserHandler(
    IUserAccessor userAccessor,
    IMissionRepo repo,
    IFileManagerCacheService storage) : IQueryHandler<GetMissionDetailByUserQuery, UserMissionDetailDto>
{
    public async Task<UserMissionDetailDto> Handle(GetMissionDetailByUserQuery request, CancellationToken ct = default)
    {
        string userId = userAccessor.GetUserId();
        var data = await repo.GetDetailByUserAsync(userId, request.Id, ct);
        var missionRewards = await Task.WhenAll(
            data.MissionRewards.Select(async item =>
            {
                var dto = item.Adapt<UserMissionRewardDetailDto>();
                dto.ItemIconUrl = item.ItemIcon is null
                    ? null
                    : await storage.GetFileUrl(item.ItemIcon, ct);

                return dto;
            }));

        var missionDto = data.Adapt<UserMissionDetailDto>();
        missionDto.MissionRewards = missionRewards;
        return missionDto;
    }
}