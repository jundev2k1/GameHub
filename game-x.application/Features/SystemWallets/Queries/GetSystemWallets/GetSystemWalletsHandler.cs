using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.SystemWallets.DTOs;

namespace game_x.application.Features.SystemWallets.Queries.GetSystemWallets;

public sealed class GetSystemWalletsHandler(
    ISystemWalletRepo systemWalletRepo) : IQueryHandler<GetSystemWalletsQuery, SystemWalletDto[]>
{
    public async Task<SystemWalletDto[]> Handle(GetSystemWalletsQuery request, CancellationToken ct = default)
    {
        var allWallets = await systemWalletRepo.GetAllAsync(ct);
        return [.. allWallets.Select(w => w.Adapt<SystemWalletDto>())];
    }
}
