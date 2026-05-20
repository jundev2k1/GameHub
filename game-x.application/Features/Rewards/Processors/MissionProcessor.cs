using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Strategies.Missions;
using game_x.domain.Entities.Rewards;

namespace game_x.application.Features.Rewards.Processors;

public interface IMissionProcessor
{
    Task ProcessAsync(Mission mission, UserEvent userEvent, CancellationToken ct = default);
}

public sealed class MissionProcessor(
    IUnitOfWork unitOfWork,
    IUserMissionRepo userMissionRepo,
    IEnumerable<IMissionProgressStrategy> strategies)
    : IMissionProcessor
{
    public async Task ProcessAsync(
        Mission mission,
        UserEvent userEvent,
        CancellationToken ct = default)
    {
        if (!mission.IsActive) return;

        var strategy = strategies.FirstOrDefault(x => x.SupportedType == mission.Type);
        if (strategy is null) throw new InvalidOperationException($"No strategy for mission {mission.Type}");

        var userMission = await userMissionRepo.GetByUserAndMissionAsync(userEvent.UserId, mission.Id, ct);
        if (userMission is null)
        {
            userMission = UserMission.Create(userEvent.UserId, mission.Id);
            await userMissionRepo.AddAsync(userMission, ct);
        }
        await strategy.ProcessAsync(mission, userMission, userEvent, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}