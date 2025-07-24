using game_x.application.Features.CounterManagement.Dtos;

namespace game_x.application.Features.CounterManagement.Admin.Queries.GetCounterDetail;

public record GetCounterDetailQuery(Guid CounterId) : IQuery<CounterDto>;
