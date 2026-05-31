using game_x.application.Features.BankAccountVerifications.Dtos;
using game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountByCriteria;
using game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountProfile;

namespace game_x.application.Features.BankAccountVerifications.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<UserBankAccount, GetBankAccountProfileResult>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CurrencyCode, src => src.FiatCurrency != null ? src.FiatCurrency.Code.Value : string.Empty)
            .Map(dest => dest.ReviewedBy, src => src.ReviewedBy != null ? src.ReviewedBy.UserName : null)
            .Map(dest => dest.ImageName, src => src.Image != null ? src.Image.FileName : string.Empty);

        cfg.NewConfig<UserBankAccount, BankAccountProfileDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CurrencyCode, src => src.FiatCurrency != null ? src.FiatCurrency.Code.Value : string.Empty)
            .Map(dest => dest.CurrencySymbol, src => src.FiatCurrency != null ? src.FiatCurrency.Symbol : string.Empty)
            .Map(dest => dest.ReviewedBy, src => src.ReviewedBy != null ? src.ReviewedBy.UserName : null)
            .Map(dest => dest.ImageName, src => src.Image != null ? src.Image.FileName : string.Empty);

        cfg.NewConfig<UserBankAccount, BankAccountListItemDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CurrencyCode, src => src.FiatCurrency != null ? src.FiatCurrency.Code.Value : string.Empty)
            .Map(dest => dest.CurrencySymbol, src => src.FiatCurrency != null ? src.FiatCurrency.Symbol : string.Empty)
            .Map(dest => dest.ReviewedBy, src => src.ReviewedBy != null ? src.ReviewedBy.UserName : null);

        cfg.NewConfig<UserBankAccount, GetBankAccountByCriteriaSearchItem>()
            .MapWith(dest => new GetBankAccountByCriteriaSearchItem(
                dest.PublicId,
                dest.UserId,
                dest.User.Email ?? string.Empty,
                dest.User.Nickname,
                dest.FiatCurrency != null ? dest.FiatCurrency.Code.Value : string.Empty,
                dest.FiatCurrency != null ? dest.FiatCurrency.Symbol : string.Empty,
                dest.Status,
                dest.SubmittedAt,
                dest.DateReviewed,
                dest.ReviewedBy != null ? dest.ReviewedBy.UserName : null));
    }
}
