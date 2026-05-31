using game_x.application.Features.TalentWallets.DTOs;

namespace game_x.application.Features.TalentWallets.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<TalentWalletTransaction, TalentWalletTransactionDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.TalentReceive, src => src.Amount);
    }
}
