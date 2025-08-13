using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.User.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<KycStatus, VerificationStatus>()
            .MapWith(src => (VerificationStatus)src);

        cfg.NewConfig<UserBankAccountStatus, VerificationStatus>()
            .MapWith(src => (VerificationStatus)src);

        cfg.NewConfig<UserKyc?, VerificationStatusDto>()
            .Map(dest => dest.Type, src => VerificationStatusType.Kyc)
            .Map(dest => dest.Status, src => src == null
                ? VerificationStatus.NotSubmitted
                : src.Status.Adapt<VerificationStatus>())
            .Map(dest => dest.IsVerified, src => (src != null) && (src.Status == KycStatus.Approved))
            .Map(dest => dest.RejectionReason, src => src != null ? src.RejectionReason : null)
            .Map(dest => dest.RejectDetails, src => src != null ? src.RejectDetails : null);

        cfg.NewConfig<UserBankAccount, VerificationStatusDto>()
            .Map(dest => dest.CurrencyCode, src => src.FiatCurrency.Code.Value)
            .Map(dest => dest.Type, src => VerificationStatusType.BankAccount)
            .Map(dest => dest.Status, src => src.Status.Adapt<VerificationStatus>())
            .Map(dest => dest.IsVerified, src => (src != null) && (src.Status == UserBankAccountStatus.Approved))
            .Map(dest => dest.RejectionReason, src => src.RejectionReason)
            .Map(dest => dest.RejectDetails, src => src.RejectDetails);
    }
}
