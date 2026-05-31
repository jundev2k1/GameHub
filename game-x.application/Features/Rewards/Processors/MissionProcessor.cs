using game_x.application.Contract.Infrastructure.SignalR.Dtos.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Events.Rewards.OnDepositMissionCompleted;
using game_x.application.Features.Rewards.Strategies.Missions;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Processors;

public interface IMissionProcessor
{
    Task ProcessAsync(Mission mission, UserEvent userEvent, CancellationToken ct = default);
}

public sealed class MissionProcessor(
    IUnitOfWork unitOfWork,
    IUserMissionRepo userMissionRepo,
    IEnumerable<IMissionProgressStrategy> strategies,
    IApplicationEventDispatcher dispatcher)
    : IMissionProcessor
{
    public async Task ProcessAsync(
        Mission mission,
        UserEvent userEvent,
        CancellationToken ct = default)
    {
        if (!mission.IsActive) return;

        var strategy = strategies.FirstOrDefault(x => x.SupportedType == userEvent.Type);
        if (strategy is null) throw new InvalidOperationException($"No strategy for mission {mission.Type}");

        var userMission = await userMissionRepo.GetTrackedByUserAndMissionAsync(userEvent.UserId, mission.Id, ct);
        if (userMission != null && userMission.Status != UserMissionStatus.InProgress)
            return;
        
        if (userMission is null)
        {
            userMission = UserMission.Create(userEvent.UserId, mission.Id);
            await userMissionRepo.AddAsync(userMission, ct);
        }
        await strategy.ProcessAsync(mission, userMission, userEvent, ct);
        await unitOfWork.SaveChangesAsync(ct);
        if (userMission.Status == UserMissionStatus.Completed)
        {
            switch (mission.Type)
            {
                case MissionType.Deposit:
                case MissionType.DepositAccumulation:
                case MissionType.Share:
                {
                    var missionDto = mission.Adapt<MissionSignalDto>();
                    await dispatcher.Publish(new OnDepositMissionCompletedEvent(userEvent.UserId, missionDto), ct);
                    break;
                }
            }
        }
    }
}