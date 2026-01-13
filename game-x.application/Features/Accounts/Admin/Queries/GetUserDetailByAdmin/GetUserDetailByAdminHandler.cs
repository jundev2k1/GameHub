using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.User.Dtos;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace game_x.application.Features.Accounts.Admin.Queries.GetUserDetailByAdmin;

public sealed class GetUserDetailByAdminHandler(
    IUserRepo userRepo,
    IGamePlatformBalanceRepo platformBalanceRepo,
    IAppLogger<GetUserDetailByAdminHandler> logger) : IQueryHandler<GetUserDetailByAdminQuery, GetUserDetailByAdminResult>
{
    public async Task<GetUserDetailByAdminResult> Handle(GetUserDetailByAdminQuery request, CancellationToken ct = default)
    {
        logger.LogInformation("1");
        var userDetail = await userRepo.GetUserDetailAsync(request.UserId, ct);
        logger.LogInformation("2");
        logger.LogInformation(JsonSerializer.Serialize(userDetail));
        var externalBalances = await platformBalanceRepo.GetBalancesByUserIdAsync(userDetail.UserId, ct);
        var result = userDetail.Adapt<GetUserDetailByAdminResult>() with
        {
            ExternalBalances = externalBalances.Adapt<UserWalletExternalItemDto[]>(),
        };
        logger.LogInformation("3");
        logger.LogInformation(JsonSerializer.Serialize(result));
        return result;
    }
}
