using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.application.Features.Accounts.User.Queries.GetSelfUser;
using UserEntity = game_x.domain.Entities.User;

namespace game_x.application.Features.Accounts.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<UserBalance, BalanceInfo>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol);

        cfg.NewConfig<UserEntity, UserDetailDto>()
            .Map(dest => dest.UserId, src => src.Id)
            .Map(dest => dest.Username, src => src.UserName)
            .Map(
                dest => dest.FullName,
                src => src.UserKyc != null ? src.UserKyc.FullName : string.Empty)
            .Map(
                dest => dest.ResidentialAddress,
                src => src.UserKyc != null ? src.UserKyc.ResidentialAddress : string.Empty)
            .Map(
                dest => dest.DateOfBirth,
                src => src.UserKyc != null ? src.UserKyc.DateOfBirth : (DateTime?)null)
            .Map(
                dest => dest.Balances,
                src => src.UserBalances.Select(ub => ub.Adapt<BalanceInfo>()).ToArray())
            .Map(
                dest => dest.UserExtendInfo,
                src => src.UserExtend.Adapt<UserExtendDto>())
            .Map(
                dest => dest.IsEmailConfirmed,
                src => src.EmailConfirmed)
            .Map(
                dest => dest.IsKycConfirmed,
                src => src.UserKyc != null && src.UserKyc.Status == KycStatus.Approved)
            .Map(
                dest => dest.IsBankConfirmed,
                src => src.UserBankAccounts != null
                    && src.UserBankAccounts.Any(uba => uba.Status == UserBankAccountStatus.Approved));

        cfg.NewConfig<UserDetailDto, GetSelfUserResult>()
            .Map(dest => dest.Roles, src => src.Roles.Items);

        cfg.NewConfig<UserBalance, UserWalletInternalItemDto>()
            .Map(dest => dest.WalletId, src => src.PublicId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId)
            .Map(dest => dest.Network, src => src.CryptoToken.Network)
            .Map(dest => dest.Symbol, src => src.CryptoToken.Symbol);
    }
}
