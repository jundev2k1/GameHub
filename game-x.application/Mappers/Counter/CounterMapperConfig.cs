using game_x.application.Features.CounterManagement.Dtos;
using CounterEntity = game_x.domain.Entities.Counter;

namespace game_x.application.Mappers.Counter;

public sealed class CounterMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<CounterEntity, CounterDto>()
            .Map(dest => dest.CounterId, src => src.PublicId)
            .Map(dest => dest.CounterNumber, src => src.CounterNumber.Value)
            .Map(dest => dest.CounterToken, src => src.CounterToken.Token);
    }
}
