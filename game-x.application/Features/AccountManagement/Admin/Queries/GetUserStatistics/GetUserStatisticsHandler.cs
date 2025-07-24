using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AccountManagement.Dtos;

namespace game_x.application.Features.AccountManagement.Admin.Queries.GetUserStatistics;

public sealed class GetUserStatisticsHandler(IUserRepo userRepo) : IQueryHandler<GetUserStatisticsQuery, UserStatisticsDto>
{
    public async Task<UserStatisticsDto> Handle(GetUserStatisticsQuery request, CancellationToken ct = default)
    {
        var statistics = await userRepo.GetUserStatisticsAsync(ct);
        return statistics;
    }
}
