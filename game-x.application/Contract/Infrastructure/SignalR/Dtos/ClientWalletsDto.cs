using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record ClientWalletsDto(
    UserWalletInternalItemDto[] InternalWallets,
    UserWalletExternalItemDto[] ExternalWallets);