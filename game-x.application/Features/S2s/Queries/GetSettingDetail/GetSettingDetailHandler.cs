using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.S2s.DTOs;

namespace game_x.application.Features.S2s.Queries.GetSettingDetail;

public sealed class GetSettingDetailHandler(IS2sClientSettingRepo s2SClientSettingRepo) : IQueryHandler<GetSettingDetailQuery, S2sClientSettingDetailDto>
{
    public async Task<S2sClientSettingDetailDto> Handle(GetSettingDetailQuery request, CancellationToken ct = default)
    {
        var result = await s2SClientSettingRepo.GetDetailAsync(request.AppCode, ct);
        if (result.ClientId != request.ClientId)
            throw new NotFoundException(nameof(request.ClientId), request.ClientId);

        return result;
    }
}
