using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.Accounts.User.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<KycStatus, VerificationStatus>()
            .MapWith(src => (VerificationStatus)src);

        cfg.NewConfig<UserBankAccountStatus, VerificationStatus>()
            .MapWith(src => (VerificationStatus)src);

        cfg.NewConfig<UserKyc, VerificationStatusDto>()
            .Map(dest => dest.Type, src => VerificationStatusType.Kyc)
            .Map(dest => dest.Status, src => src.Status.Adapt<VerificationStatus>())
            .Map(dest => dest.IsVerified, src => src.Status == KycStatus.Approved);

        cfg.NewConfig<UserBankAccount, VerificationStatusDto>()
            .Map(dest => dest.CurrencyCode, src => src.FiatCurrency.Code.Value)
            .Map(dest => dest.Type, src => VerificationStatusType.BankAccount)
            .Map(dest => dest.Status, src => src.Status.Adapt<VerificationStatus>())
            .Map(dest => dest.IsVerified, src => (src != null) && (src.Status == UserBankAccountStatus.Approved));
    }
}
