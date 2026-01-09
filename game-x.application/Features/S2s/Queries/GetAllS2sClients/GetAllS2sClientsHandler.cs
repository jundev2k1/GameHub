using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.S2s.DTOs;

namespace game_x.application.Features.S2s.Queries.GetAllS2sClients;

public sealed class GetAllS2sClientsHandler(
    IS2sClientRepo s2SClientRepo) : IQueryHandler<GetAllS2sClientsQuery, S2sClientDto[]>
{
    public async Task<S2sClientDto[]> Handle(GetAllS2sClientsQuery request, CancellationToken ct= default)
    {
        var data = await s2SClientRepo.GetAllAsync(ct);
        return data.Adapt<S2sClientDto[]>();
    }
}
