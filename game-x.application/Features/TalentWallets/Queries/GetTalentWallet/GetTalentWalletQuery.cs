namespace game_x.application.Features.TalentWallets.Queries.GetTalentWallet;

public record GetTalentWalletQuery : IQuery<GetTalentWalletResult>;

public record GetTalentWalletResult(decimal Balance);
