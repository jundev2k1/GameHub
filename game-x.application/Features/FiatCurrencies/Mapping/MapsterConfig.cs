using game_x.application.Features.FiatCurrencies.Dtos;

namespace game_x.application.Features.FiatCurrencies.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<FiatCurrency, FiatCurrencyDto>()
            .Map(src => src.Id, dest => dest.PublicId)
            .Map(src => src.Code, dest => dest.Code.Value);
    }
}
