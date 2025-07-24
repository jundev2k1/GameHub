using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.CounterManagement.Dtos;

namespace game_x.application.Features.CounterManagement.Admin.Queries.GetCounterDetail;

public sealed class GetCounterDetailHandler(ICounterRepo counterRepo) : IQueryHandler<GetCounterDetailQuery, CounterDto>
{
    public async Task<CounterDto> Handle(GetCounterDetailQuery request, CancellationToken ct = default)
    {
        var targetCounter = await counterRepo.GetByIdAsync(request.CounterId, ct);
        return targetCounter.Adapt<CounterDto>();
    }
}
