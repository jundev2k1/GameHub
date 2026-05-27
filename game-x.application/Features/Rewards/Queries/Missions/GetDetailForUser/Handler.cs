using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Features.Rewards.Queries.Missions.GetDetailForUser;

public sealed class GetMissionDetailForUserHandler(
    IUserAccessor userAccessor,
    IMissionRepo repo,
    IFileManagerCacheService storage) : IQueryHandler<GetMissionDetailForUserQuery, MissionUserDto>
{
    public async Task<MissionUserDto> Handle(GetMissionDetailForUserQuery request, CancellationToken ct = default)
    {
        string userId = userAccessor.GetUserId();
        var data = await repo.GetDetailForUserAsync(userId, request.Id, ct);
        var missionRewards = await Task.WhenAll(
            data.MissionRewards.Select(async item =>
            {
                var dto = item.Adapt<MissionRewardUserDto>();
                dto.ItemIconUrl = item.ItemIcon is null
                    ? null
                    : await storage.GetFileUrl(item.ItemIcon, ct);

                return dto;
            }));

        var missionDto = data.Adapt<MissionUserDto>();
        missionDto.MissionRewards = missionRewards;
        return missionDto;
    }
}