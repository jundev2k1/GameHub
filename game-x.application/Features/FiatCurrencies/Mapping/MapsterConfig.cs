using game_x.application.Features.FiatCurrencies.Dtos;

namespace game_x.application.Features.FiatCurrencies.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<FiatCurrency, FiatCurrencyDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Code, src => src.Code.Value);
    }
}
