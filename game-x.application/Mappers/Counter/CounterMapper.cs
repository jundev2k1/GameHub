using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.CounterManagement.Dtos;
using CounterEntity = game_x.domain.Entities.Counter;

namespace game_x.application.Mappers.Counter;

public sealed class CounterMapper()
{
    public PaginationResult<CounterDto> ToCounterDtos(PaginationResult<CounterEntity> data)
    {
        var result = new PaginationResult<CounterDto>(
            data.Items.Select(item => item.Adapt<CounterDto>()).ToList(),
            data.TotalItems,
            data.TotalPages,
            data.PageNumber,
            data.PageSize);
        return result;
    }
}
