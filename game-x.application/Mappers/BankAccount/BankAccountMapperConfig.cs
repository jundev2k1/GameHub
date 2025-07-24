using game_x.application.Features.BankAccountManagement.Dtos;
using BankAccountEntity = game_x.domain.Entities.BankAccount;


namespace game_x.application.Mappers.BankAccount;

public sealed class BankAccountMapperConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<BankAccountEntity, BankAccountDetailDto>()
            .Map(dest => dest.CurrencyCode, src => src.CurrencyCode.Value)
            .Map(dest => dest.AccountType, src => src.AccountType.Value)
            .Map(dest => dest.Status, src => src.Status.Value);
    }
}
